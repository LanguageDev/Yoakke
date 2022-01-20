// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

/// <summary>
/// Base-class for the grammar syntax-tree nodes.
/// </summary>
internal abstract partial class BnfAst
{
    /// <summary>
    /// Substitutes a node by reference.
    /// </summary>
    /// <param name="find">The node to substitute.</param>
    /// <param name="replaceWith">The node to substituite <paramref name="find"/> for.</param>
    /// <returns>A node that has all <paramref name="find"/> nodes substituted to
    /// <paramref name="replaceWith"/>, by reference.</returns>
    public BnfAst SubstituteByReference(BnfAst find, BnfAst replaceWith) => ReferenceEquals(this, find)
        ? replaceWith
        : this.SubstituteByReferenceImpl(find, replaceWith);

    /// <summary>
    /// Implements <see cref="SubstituteByReference(BnfAst, BnfAst)"/>, when this instance is not equal to
    /// <paramref name="replaceWith"/>.
    /// </summary>
    /// <param name="find">The node to substitute.</param>
    /// <param name="replaceWith">The node to substituite <paramref name="find"/> for.</param>
    /// <returns>A node that has all <paramref name="find"/> nodes substituted to
    /// <paramref name="replaceWith"/>, by reference.</returns>
    protected abstract BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith);

    /// <summary>
    /// Retrieves the first <see cref="Call"/>s this node makes, if any. Useful for left-recursion detection.
    /// </summary>
    /// <returns>The sequence of <see cref="Call"/>s that the node can make when matching without doing anything else
    /// previously.</returns>
    public abstract IEnumerable<Call> GetFirstCalls();

    /// <summary>
    /// Desugars the AST into simpler elements.
    ///
    /// The order of elements from top level to lower levels is Alt, Transform, Seq and finally everything else.
    /// </summary>
    /// <returns>The desugared <see cref="BnfAst"/>.</returns>
    public abstract BnfAst Desugar();

    /// <summary>
    /// Calculates what the result type would be, if parsing the thing this AST describes.
    /// </summary>
    /// <param name="ruleSet">The set of available rule definitions.</param>
    /// <param name="tokens">The set of available token-types.</param>
    /// <returns>The string that describes the parsed type.</returns>
    public abstract string GetParsedType(RuleSet ruleSet, TokenKindSet tokens);

    /// <inheritdoc/>
    public abstract override string ToString();
}
