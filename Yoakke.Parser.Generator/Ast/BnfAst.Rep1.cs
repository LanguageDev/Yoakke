using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Rep1 : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Rep1(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Rep1 rep
                && Subexpr.Equals(rep.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Rep1(Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{TypeNames.IList}<{Subexpr.GetParsedType(ruleSet, tokens)}>";
        }
    }
}
