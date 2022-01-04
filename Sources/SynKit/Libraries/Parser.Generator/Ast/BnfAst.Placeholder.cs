// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;

namespace Yoakke.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents the placeholder where the fold has to be fed back to.
    /// </summary>
    public class Placeholder : BnfAst
    {
        /// <summary>
        /// The referenced AST. Mainly to be able to deduce type.
        /// </summary>
        public BnfAst Reference { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Placeholder"/> class.
        /// </summary>
        /// <param name="reference">The referenced AST.</param>
        public Placeholder(BnfAst reference)
        {
            this.Reference = reference;
        }

        /// <inheritdoc/>
        public override BnfAst Desugar() => this;

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => Enumerable.Empty<Call>();

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            this.Reference.GetParsedType(ruleSet, tokens);

        /// <inheritdoc/>
        public override string ToString() => "<PLACEHOLDER>";

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => this;
    }
}
