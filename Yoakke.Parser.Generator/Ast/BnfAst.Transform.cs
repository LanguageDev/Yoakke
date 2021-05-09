using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Transform : BnfAst
        {
            public readonly BnfAst Subexpr;
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

            public override BnfAst Desugar() => new Transform(Subexpr.Desugar(), Method);

            public override string GetParsedType(RuleSet ruleSet) => Method.ReturnType.ToDisplayString();
        }
    }
}
