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
                if (Elements.Count == 1) return Elements[0].Desugar();

                var newElements = new List<BnfAst>();
                void Add(Seq seq)
                {
                    foreach (var e in seq.Elements.Select(a => a.Desugar()))
                    {
                        if (e is Seq subSeq) Add(subSeq);
                        else newElements.Add(e);
                    }
                }
                Add(this);

                for (int i = 0; i < newElements.Count; ++i)
                {
                    if (newElements[i] is Alt alt)
                    {
                        // We substitute the sequence with alternations, where each in each alternative
                        // the ith sequence element is replaced by a given alternation
                        var alternatives = new List<BnfAst>();
                        foreach (var altElement in alt.Elements)
                        {
                            var newSequence = newElements.ToArray();
                            newSequence[i] = altElement;
                            alternatives.Add(new Seq(newSequence));
                        }
                        return new Alt(alternatives).Desugar();
                    }
                }
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
