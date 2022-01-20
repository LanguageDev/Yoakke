// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents a sub-rule that can be repeated 0 or more times.
    /// </summary>
    public class Rep0 : BnfAst
    {
        /// <summary>
        /// The sub-rule to repeat.
        /// </summary>
        public BnfAst Subexpr { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rep0"/> class.
        /// </summary>
        /// <param name="subexpr">The subexpression that can be repeated 0 or more times.</param>
        public Rep0(BnfAst subexpr)
        {
            this.Subexpr = subexpr;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) =>
            new Rep0(this.Subexpr.SubstituteByReference(find, replaceWith));

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => this.Subexpr.GetFirstCalls();

        /// <inheritdoc/>
        public override BnfAst Desugar() => new Rep0(this.Subexpr.Desugar());

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            $"{TypeNames.IReadOnlyList}<{this.Subexpr.GetParsedType(ruleSet, tokens)}>";

        /// <inheritdoc/>
        public override string ToString() => $"Rep0({this.Subexpr})";
    }
}
