// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
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
            public override bool Equals(BnfAst other) => other is Group group
               && this.Subexpr.Equals(group.Subexpr);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Subexpr.GetHashCode();

            /// <inheritdoc/>
            public override BnfAst Desugar() => this.Subexpr is Seq
                ? new Group(this.Subexpr.Desugar())
                : this.Subexpr.Desugar();

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                this.Subexpr.GetParsedType(ruleSet, tokens);
        }
    }
}
