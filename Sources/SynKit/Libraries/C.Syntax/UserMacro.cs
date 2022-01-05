// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// A <see cref="IMacro"/> that can be defined by the user.
/// Has optional arguments, returns it's body with the arguments substituted.
/// </summary>
public class UserMacro : IMacro
{
    private const string EmptyStringLiteral = "\"\"";

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IReadOnlyList<string>? Parameters { get; }

    private readonly IReadOnlyList<MacroElement> body;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserMacro"/> class.
    /// </summary>
    /// <param name="name">The name of the macro.</param>
    /// <param name="parameters">The list of macro parameter names, or null, if this macro needs to be invoked
    /// without parenthesis.</param>
    /// <param name="body">The list of <see cref="MacroElement"/>s that describes the expansion.</param>
    public UserMacro(string name, IReadOnlyList<string>? parameters, IReadOnlyList<MacroElement> body)
    {
        this.Name = name;
        this.Parameters = parameters;
        this.body = body;
    }

    /// <inheritdoc/>
    public IEnumerable<CToken> Expand(IPreProcessor preProcessor, IReadOnlyList<IReadOnlyList<CToken>> arguments)
    {
        var isVariadic = this.Parameters is not null
                      && this.Parameters.Count > 0
                      && this.Parameters[this.Parameters.Count - 1] == "...";
        var hasVariadicArgs = true;
        if (this.Parameters is not null && this.Parameters.Count != arguments.Count)
        {
            // This can still be ok if we are variadic and the count is only one less
            if (!isVariadic || this.Parameters.Count != arguments.Count + 1)
            {
                // TODO: Proper error handling
                throw new NotImplementedException();
            }
            if (isVariadic) hasVariadicArgs = false;
        }

        // Assign the arguments
        var argDict = new Dictionary<string, IReadOnlyList<CToken>>();
        if (this.Parameters is not null)
        {
            for (var i = 0; i < arguments.Count;)
            {
                var paramName = this.Parameters[i];
                if (paramName == "...") paramName = "__VA_ARGS__";
                // We expand each argument to conform standards
                argDict[paramName] = preProcessor.Expand(arguments[i]).ToList();
                ++i;
            }
        }

        // If we have no variadic arguments but need it, just define it as empty
        if (!hasVariadicArgs) argDict["__VA_ARGS__"] = Array.Empty<CToken>();

        // Do the substitution
        var result = new List<CToken>();
        foreach (var element in this.body) result.AddRange(ExpandElement(argDict, element));
        return result;
    }

    private static IReadOnlyList<CToken> ExpandElement(
        IReadOnlyDictionary<string, IReadOnlyList<CToken>> args,
        MacroElement element) => element switch
        {
            MacroElement.Literal lit => args.TryGetValue(lit.Token.LogicalText, out var arg)
                    ? arg
                    : new CToken[] { lit.Token },

            MacroElement.Stringify str => new CToken[] { Stringify(args[str.Argument]) },

            MacroElement.Paste paste => Paste(ExpandElement(args, paste.Left), ExpandElement(args, paste.Right)),

            _ => throw new NotImplementedException(),
        };

    private static CToken Stringify(IReadOnlyList<CToken> tokens)
    {
        // TODO: Supply proper ranges?
        if (tokens.Count == 0) return new(default, EmptyStringLiteral, default, EmptyStringLiteral, CTokenType.StringLiteral);

        var result = new StringBuilder();
        // First token just gets pasted
        result.Append(tokens[0].LogicalText);
        for (var i = 1; i < tokens.Count; ++i)
        {
            var prev = tokens[i - 1];
            var current = tokens[i];
            var touching = prev.LogicalRange.End == current.LogicalRange.Start;
            // If not touching, separate with a space
            if (!touching) result.Append(' ');
            result.Append(current.LogicalText);
        }
        // Escape quotes
        result.Replace("\"", "\\\"");
        // Add quotes around
        result.Insert(0, '"').Append('"');
        // Create the string literal value
        var str = result.ToString();
        // TODO: Supply proper ranges?
        return new(default, str, default, str, CTokenType.StringLiteral);
    }

    private static IReadOnlyList<CToken> Paste(IReadOnlyList<CToken> left, IReadOnlyList<CToken> right)
    {
        // If either of them are empty, return the other simply
        // NOTE: This handles the case when both are empty
        if (right.Count == 0) return left;
        if (left.Count == 0) return right;

        // Neither of them are empty, which means we take the last token from left, the first from right
        // and try to mush them together, then return that with all the other tokens around
        var leftLast = left[left.Count - 1];
        var rightFirst = right[0];
        // Concatenate sources
        var pastedTokenSource = new StringBuilder()
            .Append(leftLast.LogicalText)
            .Append(rightFirst.LogicalText)
            .ToString();
        // Try to lex a single token
        var lexer = new CLexer(pastedTokenSource);
        var token = lexer.Next();
        if (token.Kind == CTokenType.End)
        {
            // TODO: Proper error handling
            throw new NotImplementedException();
        }
        // TODO: The range of the token is totally bogus, reconstruct it?
        // Now expect the end
        var endToken = lexer.Next();
        if (endToken.Kind != CTokenType.End)
        {
            // TODO: Proper error handling
            throw new NotImplementedException();
        }
        // Everything is alright, concatenate results
        return left
            .Take(left.Count - 1)
            .Append(token)
            .Concat(right.Skip(1))
            .ToList();
    }
}
