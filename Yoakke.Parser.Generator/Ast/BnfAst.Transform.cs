using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Subexpr = subexpr;
                Method = method;
            }

            public override bool Equals(BnfAst other) => other is Transform tr
                && Subexpr.Equals(tr.Subexpr)
                && SymbolEqualityComparer.Default.Equals(Method, tr.Method);
            public override int GetHashCode() => HashCode.Combine(Subexpr, Method);

            public override BnfAst Desugar()
            {
                // We sink Transform under alternation
                var sub = Subexpr.Desugar();
                if (sub is Alt alt) return new Alt(alt.Elements.Select(e => new Transform(e, Method)));
                return new Transform(sub, Method);
            }

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) => 
                Method.ReturnType.ToDisplayString();
        }
    }
}
