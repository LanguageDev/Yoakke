// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.SynKit.Parser.Tests;

// https://github.com/LanguageDev/Yoakke/issues/138
public partial class Issue138Tests
{
    internal record FunctionDefinition(List<IDeclarationSpecifier> Specifiers, Declarator Declarator)
    {
        public override string ToString() =>
            $"Specifiers=[{string.Join(", ", this.Specifiers)}], Declarator={this.Declarator}";
    }

    internal interface IDeclarationSpecifier
    {
    }

    internal record Declarator(DirectDeclarator DirectDeclarator)
    {
        public override string ToString() => this.DirectDeclarator.ToString();
    }

    internal record DirectDeclarator(string Text)
    {
        public override string ToString() => this.Text;
    }

    internal record TypeSpecifier(string Name) : IDeclarationSpecifier
    {
        public override string ToString() => this.Name;
    }

    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Regex(Regexes.Identifier)] Identifier,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        // function_definition : declaration_specifiers declarator
        // declaration_specifiers : type_specifier declaration_specifiers?
        [CustomParser("function_definition")]
        private ParseResult<FunctionDefinition> customParseFunctionDefinition(int offset)
        {
            // We transform
            // declaration_specifiers : type_specifier declaration_specifiers?
            // into
            // declaration_specifiers : type_specifier+
            // They are essentially equivalent
            var firstTypeSpecifier = this.parseTypeSpecifier(offset);
            if (firstTypeSpecifier.IsError) return firstTypeSpecifier.Error;

            // Now we start collecting each type_specifier into a list, each with the offset before them
            // This will build up our 'declaration_specifiers' rule
            // We keep parsing as long as we can
            var typeSpecifiers = new List<(TypeSpecifier TypeSpecifier, int OffsetBefore)> { (firstTypeSpecifier.Ok, offset) };
            offset = firstTypeSpecifier.Ok.Offset;
            while (true)
            {
                var nextTypeSpecifier = this.parseTypeSpecifier(offset);
                if (nextTypeSpecifier.IsError) break;
                typeSpecifiers.Add((nextTypeSpecifier.Ok, offset));
                offset = nextTypeSpecifier.Ok.Offset;
            }

            // Now we try to parse a 'declarator'
            var declarator = this.parseDeclarator(offset);

            if (declarator.IsError)
            {
                // There was a problem, a declarator could not be parsed, let's start backtracking
                // NOTE: We don't try with the first one on purpose, we need at least one type specifier!
                for (var i = typeSpecifiers.Count - 1; i > 0; --i)
                {
                    // Try before consuming this type specifier
                    var (_, offs) = typeSpecifiers[i];
                    var prevDeclarator = this.parseDeclarator(offs);
                    if (prevDeclarator.IsOk)
                    {
                        // Found a good alternative
                        declarator = prevDeclarator;
                        typeSpecifiers.RemoveRange(i, typeSpecifiers.Count - i);
                        break;
                    }
                }
            }

            if (declarator.IsError)
            {
                // Still an error, could not backtrack
                return declarator.Error;
            }

            // Was able to backtrack, or was good all along
            return ParseResult.Ok(
                new FunctionDefinition(
                    typeSpecifiers.Select(s => s.TypeSpecifier).ToList<IDeclarationSpecifier>(),
                    declarator.Ok),
                    declarator.Ok.Offset,
                    declarator.Ok.FurthestError);
        }

        [Rule("declarator: direct_declarator")]
        private static Declarator MakeDeclarator(DirectDeclarator directDeclarator) =>
            new(directDeclarator);

        [Rule("direct_declarator: Identifier")]
        private static DirectDeclarator MakeDirectDeclarator(IToken identifier) =>
            new(identifier.Text);

        [Rule("type_specifier: 'int'")]
        [Rule("type_specifier: Identifier")]
        private static TypeSpecifier MakeSimpleTypeSpecifier(IToken specifier) => new(specifier.Text);
    }

    private static FunctionDefinition Parse(string source) =>
       new Parser(new Lexer(source)).ParseFunctionDefinition().Ok.Value;

    [InlineData("int main", "Specifiers=[int], Declarator=main")]
    [InlineData("unsigned int main", "Specifiers=[unsigned, int], Declarator=main")]
    [InlineData("unsigned long int main", "Specifiers=[unsigned, long, int], Declarator=main")]
    [InlineData("unsigned long long int main", "Specifiers=[unsigned, long, long, int], Declarator=main")]
    [InlineData("unsigned long long main", "Specifiers=[unsigned, long, long], Declarator=main")]
    [Theory]
    public void Tests(string input, string expected) =>
        Assert.Equal(expected, Parse(input).ToString());
}
