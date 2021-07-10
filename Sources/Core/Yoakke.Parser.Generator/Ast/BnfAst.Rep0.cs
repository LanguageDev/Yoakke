// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
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
            public readonly BnfAst Subexpr;

            public Rep0(BnfAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            /// <inheritdoc/>
            public override bool Equals(BnfAst other) => other is Rep0 rep
                && this.Subexpr.Equals(rep.Subexpr);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Subexpr.GetHashCode();

            /// <inheritdoc/>
            public override BnfAst Desugar() => new Rep0(this.Subexpr.Desugar());

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{TypeNames.IReadOnlyList}<{this.Subexpr.GetParsedType(ruleSet, tokens)}>";
        }
    }
}
