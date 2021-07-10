// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
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
            public override bool Equals(BnfAst other) => other is Opt opt
                && this.Subexpr.Equals(opt.Subexpr);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Subexpr.GetHashCode();

            /// <inheritdoc/>
            public override BnfAst Desugar() => new Opt(this.Subexpr.Desugar());

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{this.Subexpr.GetParsedType(ruleSet, tokens)}?";
        }
    }
}
