using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Alt : BnfAst
        {
            public readonly BnfAst First;
            public readonly BnfAst Second;

            public Alt(BnfAst first, BnfAst second)
            {
                First = first;
                Second = second;
            }

            public override bool Equals(BnfAst other) => other is Alt alt 
                && First.Equals(alt.First) 
                && Second.Equals(alt.Second);
            public override int GetHashCode() => HashCode.Combine(First, Second);

            public override string GetParsedType(RuleSet ruleSet)
            {
                var leftType = First.GetParsedType(ruleSet);
                var rightType = Second.GetParsedType(ruleSet);
                if (leftType != rightType) throw new InvalidOperationException("Incompatible alternative types");
                return leftType;
            }

            public IEnumerable<BnfAst> GetAlternatives()
            {
                if (First is Alt alt1)
                {
                    foreach (var e in alt1.GetAlternatives()) yield return e;
                }
                else yield return First;
                if (Second is Alt alt2)
                {
                    foreach (var e in alt2.GetAlternatives()) yield return e;
                }
                else yield return Second;
            }
        }
    }
}
