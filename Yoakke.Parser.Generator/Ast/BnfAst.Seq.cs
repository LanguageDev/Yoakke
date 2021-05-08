using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Seq : BnfAst
        {
            public readonly BnfAst First;
            public readonly BnfAst Second;

            public Seq(BnfAst first, BnfAst second)
            {
                First = first;
                Second = second;
            }

            public override bool Equals(BnfAst other) => other is Seq seq
                && First.Equals(seq.First)
                && Second.Equals(seq.Second);
            public override int GetHashCode() => HashCode.Combine(First, Second);

            public override string GetParsedType(RuleSet ruleSet) =>
                $"({First.GetParsedType(ruleSet)}, {Second.GetParsedType(ruleSet)})";

            public IEnumerable<BnfAst> GetSequence()
            {
                if (First is Seq seq1)
                {
                    foreach (var e in seq1.GetSequence()) yield return e;
                }
                else yield return First;
                if (Second is Seq seq2)
                {
                    foreach (var e in seq2.GetSequence()) yield return e;
                }
                else yield return Second;
            }
        }
    }
}
