// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.SynKit.Parser.Generator.Model;
using RuleSet = Yoakke.SynKit.Parser.Generator.Model.RuleSet;

namespace Yoakke.SynKit.Parser.Generator.Ast;

internal partial class BnfAst
{
    /// <summary>
    /// A literal token match, either by text or by token kind.
    /// </summary>
    public class Literal : BnfAst
    {
        /// <summary>
        /// The value to match.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Literal"/> class.
        /// </summary>
        /// <param name="value">The literal value to match.</param>
        public Literal(object value)
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => this;

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => Enumerable.Empty<Call>();

        /// <inheritdoc/>
        public override BnfAst Desugar() => this;

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
        {
            if (tokens.EnumType == null) return TypeNames.IToken;
            else return $"{TypeNames.IToken}<{tokens.EnumType.ToDisplayString()}>";
        }

        /// <inheritdoc/>
        public override string ToString() => this.Value is IFieldSymbol field
            ? field.Name
            : $"\"{this.Value}\"";
    }
}
