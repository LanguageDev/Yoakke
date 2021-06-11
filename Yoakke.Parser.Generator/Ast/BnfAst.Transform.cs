using Microsoft.CodeAnalysis;
using System.Linq;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents a rule thats result is transformed using a method.
        /// </summary>
        public class Transform : BnfAst
        {
            /// <summary>
            /// The rule that needs its results transformed.
            /// </summary>
            public readonly BnfAst Subexpr;

            /// <summary>
            /// The transformation method.
            /// </summary>
            public readonly IMethodSymbol Method;

            public Transform(BnfAst subexpr, IMethodSymbol method)
            {
                this.Subexpr = subexpr;
                this.Method = method;
            }

            public override bool Equals(BnfAst other) => other is Transform tr
                && this.Subexpr.Equals(tr.Subexpr)
                && SymbolEqualityComparer.Default.Equals(this.Method, tr.Method);
            public override int GetHashCode() => HashCode.Combine(this.Subexpr, this.Method);

            public override BnfAst Desugar()
            {
                // We sink Transform under alternation
                var sub = this.Subexpr.Desugar();
                if (sub is Alt alt) return new Alt(alt.Elements.Select(e => new Transform(e, this.Method)));
                return new Transform(sub, this.Method);
            }

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                this.Method.ReturnType.ToDisplayString();
        }
    }
}
