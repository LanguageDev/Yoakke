// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
    internal partial class BnfAst
    {
        /// <summary>
        /// A literal token match, either by text or by token kind.
        /// </summary>
        public class Literal : BnfAst
        {
            /// <summary>
            /// The value to match.
            /// </summary>
            public readonly object Value;

            public Literal(object value)
            {
                this.Value = value;
            }

            /// <inheritdoc/>
            public override bool Equals(BnfAst other) => other is Literal lit
                && this.Value.Equals(lit.Value);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Value.GetHashCode();

            /// <inheritdoc/>
            public override BnfAst Desugar() => this;

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                if (tokens.EnumType == null) return TypeNames.IToken;
                else return $"{TypeNames.IToken}<{tokens.EnumType.ToDisplayString()}>";
            }
        }
    }
}
