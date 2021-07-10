// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    internal partial class BnfAst
    {
        /// <summary>
        /// Represents a rule thats result is transformed using a method.
        /// </summary>
        public class Transform : BnfAst
        {
            /// <summary>
            /// The rule that needs its results transformed.
            /// </summary>
            public BnfAst Subexpr { get; }

            /// <summary>
            /// The transformation method.
            /// </summary>
            public IMethodSymbol Method { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Transform"/> class.
            /// </summary>
            /// <param name="subexpr">The subexpression to match.</param>
            /// <param name="method">The method to transform the result with.</param>
            public Transform(BnfAst subexpr, IMethodSymbol method)
            {
                this.Subexpr = subexpr;
                this.Method = method;
            }

            /// <inheritdoc/>
            public override bool Equals(BnfAst other) => other is Transform tr
                && this.Subexpr.Equals(tr.Subexpr)
                && SymbolEqualityComparer.Default.Equals(this.Method, tr.Method);

            /// <inheritdoc/>
            public override int GetHashCode() => HashCode.Combine(this.Subexpr, this.Method);

            /// <inheritdoc/>
            public override BnfAst Desugar()
            {
                // We sink Transform under alternation
                var sub = this.Subexpr.Desugar();
                if (sub is Alt alt) return new Alt(alt.Elements.Select(e => new Transform(e, this.Method)));
                return new Transform(sub, this.Method);
            }

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                this.Method.ReturnType.ToDisplayString();
        }
    }
}
