using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Rep0 : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Rep0(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Rep0 rep
                && Subexpr.Equals(rep.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();
        }
    }
}
