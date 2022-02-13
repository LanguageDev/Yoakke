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
    /// Represents a method call.
    /// </summary>
    public class MethodCall : BnfAst
    {
        /// <summary>
        /// The called method.
        /// </summary>
        public IMethodSymbol Method { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCall"/> class.
        /// </summary>
        /// <param name="method">The called method.</param>
        public MethodCall(IMethodSymbol method)
        {
            this.Method = method;
        }

        /// <inheritdoc/>
        protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => this;

        /// <inheritdoc/>
        public override IEnumerable<Call> GetFirstCalls() => Enumerable.Empty<Call>();

        /// <inheritdoc/>
        public override BnfAst Desugar() => this;

        /// <inheritdoc/>
        public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
            // NOTE: We assume ParseResult<T>
            ((INamedTypeSymbol)this.Method.ReturnType).TypeArguments[0].ToDisplayString();

        /// <inheritdoc/>
        public override string ToString() => $"MethodCall({this.Method.Name})";
    }
}
