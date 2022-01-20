// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents alternative sub-rules to parse.
    /// </summary>
    public class Alt : BnfAst
    {
        /// <summary>
        /// The list of alternative rules.
        /// </summary>
        public IReadOnlyList<BnfAst> Elements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alt"/> class.
        /// </summary>
        /// <param name="first">The first alternative.</param>
        /// <param name="second">The second alternative.</param>
        public Alt(BnfAst first, BnfAst second)
        {
            this.Elements = new BnfAst[] { first, second };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alt"/> class.
        /// </summary>
        /// <param name="elements">The sequence of alternatives.</param>
        public Alt(IEnumerable<BnfAst> elements)
        {
            this.Elements = elements.ToArray();
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) =>
            new Alt(this.Elements.Select(e => e.SubstituteByReference(find, replaceWith)));

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() =>
            this.Elements.SelectMany(e => e.GetFirstCalls());

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            this.Elements[0].GetParsedType(ruleSet, tokens);

        /// <inheritdoc/>
        public override string ToString() => $"Alt({string.Join(", ", this.Elements)})";
    }
}
