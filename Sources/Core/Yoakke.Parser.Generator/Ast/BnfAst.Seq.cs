// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    internal partial class BnfAst
    {
        /// <summary>
        /// Represents a sequence of rules to apply one after another.
        /// </summary>
        public class Seq : BnfAst
        {
            /// <summary>
            /// The sequence of rules.
            /// </summary>
            public readonly IReadOnlyList<BnfAst> Elements;

            public Seq(BnfAst first, BnfAst second)
            {
                this.Elements = new BnfAst[] { first, second };
            }

            public Seq(IEnumerable<BnfAst> elements)
            {
                this.Elements = elements.ToArray();
            }

            /// <inheritdoc/>
            public override bool Equals(BnfAst other) => other is Seq seq
                && this.Elements.SequenceEqual(seq.Elements);

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                var hash = default(HashCode);
                foreach (var e in this.Elements) hash.Add(e);
                return hash.ToHashCode();
            }

            /// <inheritdoc/>
            public override BnfAst Desugar()
            {
                if (this.Elements.Count == 1) return this.Elements[0].Desugar();

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

                for (var i = 0; i < newElements.Count; ++i)
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

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                if (this.Elements.Count == 1) return this.Elements[0].GetParsedType(ruleSet, tokens);
                return $"({string.Join(", ", this.Elements.Select(e => e.GetParsedType(ruleSet, tokens)))})";
            }
        }
    }
}
