// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents alternative sub-rules to parse.
        /// </summary>
        public class Alt : BnfAst
        {
            /// <summary>
            /// The list of alternative rules.
            /// </summary>
            public readonly IReadOnlyList<BnfAst> Elements;

            public Alt(BnfAst first, BnfAst second)
            {
                this.Elements = new BnfAst[] { first, second };
            }

            public Alt(IEnumerable<BnfAst> elements)
            {
                this.Elements = elements.ToArray();
            }

            public override bool Equals(BnfAst other) => other is Alt alt
                && this.Elements.SequenceEqual(alt.Elements);

            public override int GetHashCode()
            {
                var hash = new HashCode();
                foreach (var e in this.Elements) hash.Add(e);
                return hash.ToHashCode();
            }

            public override BnfAst Desugar()
            {
                if (this.Elements.Count == 1) return this.Elements[0].Desugar();

                var newElements = new List<BnfAst>();
                void Add(Alt alt)
                {
                    foreach (var e in alt.Elements.Select(a => a.Desugar()))
                    {
                        if (e is Alt subAlt) Add(subAlt);
                        else newElements.Add(e);
                    }
                }
                Add(this);
                return new Alt(newElements);
            }

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                this.Elements[0].GetParsedType(ruleSet, tokens);
        }
    }
}
