// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents a grouped sub-rule application that won't be unwrapped when transformed.
    /// </summary>
    public class Group : BnfAst
    {
        /// <summary>
        /// The sub-rule that won't be unwrapped when transformed.
        /// </summary>
        public BnfAst Subexpr { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="subexpr">The grouped subexpression.</param>
        public Group(BnfAst subexpr)
        {
            this.Subexpr = subexpr;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) =>
            new Group(this.Subexpr.SubstituteByReference(find, replaceWith));

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => this.Subexpr.GetFirstCalls();

        /// <inheritdoc/>
        public override BnfAst Desugar() => this.Subexpr is Seq
            ? new Group(this.Subexpr.Desugar())
            : this.Subexpr.Desugar();

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            this.Subexpr.GetParsedType(ruleSet, tokens);

        /// <inheritdoc/>
        public override string ToString() => $"[{this.Subexpr}]";
    }
}
