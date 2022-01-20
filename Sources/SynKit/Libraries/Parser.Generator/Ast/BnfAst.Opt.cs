// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents an optional sub-rule application.
    /// </summary>
    public class Opt : BnfAst
    {
        /// <summary>
        /// The optional sub-rule.
        /// </summary>
        public BnfAst Subexpr { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opt"/> class.
        /// </summary>
        /// <param name="subexpr">The optional subexpression.</param>
        public Opt(BnfAst subexpr)
        {
            this.Subexpr = subexpr;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) =>
            new Opt(this.Subexpr.SubstituteByReference(find, replaceWith));

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => this.Subexpr.GetFirstCalls();

        /// <inheritdoc/>
        public override BnfAst Desugar() => new Opt(this.Subexpr.Desugar());

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            $"{this.Subexpr.GetParsedType(ruleSet, tokens)}?";

        /// <inheritdoc/>
        public override string ToString() => $"Opt({this.Subexpr})";
    }
}
