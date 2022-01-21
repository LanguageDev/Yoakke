// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// Represents parsing a sub-rule that is referenced by name.
    /// </summary>
    public class Call : BnfAst
    {
        /// <summary>
        /// The name of the sub-rule.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Call"/> class.
        /// </summary>
        /// <param name="name">The name of the sub-rule to call.</param>
        public Call(string name)
        {
            this.Name = name;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => this;

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => new[] { this };

        /// <inheritdoc/>
        public override BnfAst Desugar() => this;

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
        {
            var called = ruleSet.GetRule(this.Name);
            return called.Ast.GetParsedType(ruleSet, tokens);
        }

        /// <inheritdoc/>
        public override string ToString() => this.Name;
    }
}
