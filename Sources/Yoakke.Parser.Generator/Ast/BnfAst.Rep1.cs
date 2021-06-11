// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
    internal partial class BnfAst
    {
        /// <summary>
        /// Represents a sub-rule that can be repeated 1 or more times.
        /// </summary>
        public class Rep1 : BnfAst
        {
            /// <summary>
            /// The sub-rule to repeat.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Rep1(BnfAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Rep1 rep
                && this.Subexpr.Equals(rep.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Rep1(this.Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{TypeNames.IReadOnlyList}<{this.Subexpr.GetParsedType(ruleSet, tokens)}>";
        }
    }
}
