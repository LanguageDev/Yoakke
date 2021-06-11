// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents a grouped sub-rule application that won't be unwrapped when transformed.
        /// </summary>
        public class Group : BnfAst
        {
            /// <summary>
            /// The sub-rule that won't be unwrapped when transformed.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Group(BnfAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Group group
               && this.Subexpr.Equals(group.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override BnfAst Desugar() => this.Subexpr is Seq
                ? new Group(this.Subexpr.Desugar())
                : this.Subexpr.Desugar();

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                this.Subexpr.GetParsedType(ruleSet, tokens);
        }
    }
}
