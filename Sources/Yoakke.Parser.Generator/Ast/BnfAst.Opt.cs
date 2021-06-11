// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents an optional sub-rule application.
        /// </summary>
        public class Opt : BnfAst
        {
            /// <summary>
            /// The optional sub-rule.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Opt(BnfAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Opt opt
                && this.Subexpr.Equals(opt.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Opt(this.Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{this.Subexpr.GetParsedType(ruleSet, tokens)}?";
        }
    }
}
