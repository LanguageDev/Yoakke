using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Seq : BnfAst
        {
            public readonly IReadOnlyList<BnfAst> Elements;

            public Seq(BnfAst first, BnfAst second)
            {
                Elements = new BnfAst[] { first, second };
            }

            public Seq(IEnumerable<BnfAst> elements)
            {
                Elements = elements.ToArray();
            }

            public override bool Equals(BnfAst other) => other is Seq seq
                && Elements.SequenceEqual(seq.Elements);
            public override int GetHashCode()
            {
                var hash = new HashCode();
                foreach (var e in Elements) hash.Add(e);
                return hash.ToHashCode();
            }

            public override BnfAst Desugar()
            {
                var newElements = new List<BnfAst>();

                void Add(Seq seq)
                {
                    foreach (var e in seq.Elements)
                    {
                        if (e is Seq subSeq) Add(subSeq);
                        else newElements.Add(e);
                    }
                }

                Add(this);
                return new Seq(newElements);
            }

            public override string GetParsedType(RuleSet ruleSet)
            {
                if (Elements.Count == 1) return Elements[0].GetParsedType(ruleSet);
                return $"({string.Join(", ", Elements.Select(e => e.GetParsedType(ruleSet)))})";
            }
        }
    }
}
