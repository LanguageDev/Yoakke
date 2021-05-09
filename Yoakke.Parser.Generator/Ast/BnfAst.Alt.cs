using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Alt : BnfAst
        {
            public readonly IReadOnlyList<BnfAst> Elements;

            public Alt(BnfAst first, BnfAst second)
            {
                Elements = new BnfAst[] { first, second };
            }

            public Alt(IEnumerable<BnfAst> elements)
            {
                Elements = elements.ToArray();
            }

            public override bool Equals(BnfAst other) => other is Alt alt 
                && Elements.SequenceEqual(alt.Elements);
            public override int GetHashCode()
            {
                var hash = new HashCode();
                foreach (var e in Elements) hash.Add(e);
                return hash.ToHashCode();
            }

            public override BnfAst Desugar()
            {
                var newElements = new List<BnfAst>();

                void Add(Alt alt)
                {
                    foreach (var e in alt.Elements)
                    {
                        if (e is Alt subAlt) Add(subAlt);
                        else newElements.Add(e.Desugar());
                    }
                }

                Add(this);
                return new Alt(newElements);
            }

            public override string GetParsedType(RuleSet ruleSet) => Elements[0].GetParsedType(ruleSet);
        }
    }
}
