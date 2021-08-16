// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    internal partial class BnfAst
    {
        /// <summary>
        /// Represents a repeating, folding parse rule.
        /// This is used for left-recursion elimination.
        /// </summary>
        public class FoldLeft : BnfAst
        {
            /// <summary>
            /// The sub-element to apply.
            /// </summary>
            public BnfAst First { get; }

            /// <summary>
            /// The alternative elements to apply repeatedly after.
            /// </summary>
            public IReadOnlyList<(BnfAst Node, IMethodSymbol Method)> Seconds { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="FoldLeft"/> class.
            /// </summary>
            /// <param name="first">The first element of the fold.</param>
            /// <param name="seconds">The second elements and transformations of the fold, that will be repeated.</param>
            public FoldLeft(BnfAst first, IReadOnlyList<(BnfAst Node, IMethodSymbol Method)> seconds)
            {
                this.First = first;
                this.Seconds = seconds;
            }

            /// <inheritdoc/>
            protected override BnfAst SubstituteByReferenceImpl(BnfAst find, BnfAst replaceWith) => new FoldLeft(
                this.First.SubstituteByReference(find, replaceWith),
                this.Seconds.Select(s => (s.Node.SubstituteByReference(find, replaceWith), s.Method)).ToList());

            /// <inheritdoc/>
            public override IEnumerable<Call> GetFirstCalls() => this.First.GetFirstCalls();

            /// <inheritdoc/>
            public override BnfAst Desugar() => new FoldLeft(
                this.First.Desugar(),
                this.Seconds.Select(s => (s.Node.Desugar(), s.Method)).ToList());

            /// <inheritdoc/>
            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                var firstType = this.First.GetParsedType(ruleSet, tokens);
                foreach (var (_, method) in this.Seconds)
                {
                    var mappedType = method.ReturnType.ToDisplayString();
                    if (firstType != mappedType) throw new InvalidOperationException("Incompatible folded types");
                }
                return firstType;
            }

            /// <inheritdoc/>
            public override string ToString() => $"FoldLeft({this.First}, {this.Seconds})";
        }
    }
}
