using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Opt : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Opt(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Opt opt
                && Subexpr.Equals(opt.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override string GetParsedType(RuleSet ruleSet) => $"{Subexpr.GetParsedType(ruleSet)}?";
        }
    }
}
