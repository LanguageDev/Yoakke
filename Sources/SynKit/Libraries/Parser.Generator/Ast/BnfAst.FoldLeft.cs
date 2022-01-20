// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents a repeating, folding parse rule.
    /// This is used for left-recursion elimination.
    /// </summary>
    public class FoldLeft : BnfAst
    {
        /// <summary>
        /// The sub-element to apply.
        /// </summary>
        public BnfAst First { get; }

        /// <summary>
        /// The alternative elements to apply repeatedly after.
        /// </summary>
        public BnfAst Second { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FoldLeft"/> class.
        /// </summary>
        /// <param name="first">The first element of the fold.</param>
        /// <param name="second">The second elements of the fold, that will be repeated.</param>
        public FoldLeft(BnfAst first, BnfAst second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => new FoldLeft(
            this.First.SubstituteByReference(find, replaceWith),
            this.Second.SubstituteByReference(find, replaceWith));

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => this.First.GetFirstCalls();

        /// <inheritdoc/>
        public override BnfAst Desugar() =>
            new FoldLeft(this.First.Desugar(), this.Second.Desugar());

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            this.First.GetParsedType(ruleSet, tokens);

        /// <inheritdoc/>
        public override string ToString() => $"FoldLeft({this.First}, {this.Second})";
    }
}
