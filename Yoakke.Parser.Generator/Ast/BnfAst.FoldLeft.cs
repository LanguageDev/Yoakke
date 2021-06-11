using Microsoft.CodeAnalysis;
using System;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
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
            public readonly BnfAst First;

            /// <summary>
            /// The element to apply repeatedly after.
            /// </summary>
            public readonly BnfAst Second;

            /// <summary>
            /// The transformation method that does the folding.
            /// </summary>
            public readonly IMethodSymbol Method;

            public FoldLeft(BnfAst first, BnfAst second, IMethodSymbol method)
            {
                this.First = first;
                this.Second = second;
                this.Method = method;
            }

            public override bool Equals(BnfAst other) => other is FoldLeft fl
                && this.First.Equals(fl.First)
                && this.Second.Equals(fl.Second)
                && SymbolEqualityComparer.Default.Equals(this.Method, fl.Method);
            public override int GetHashCode() => HashCode.Combine(this.First, this.Second, this.Method);

            public override BnfAst Desugar() => new FoldLeft(this.First.Desugar(), this.Second.Desugar(), this.Method);

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                var firstType = this.First.GetParsedType(ruleSet, tokens);
                var mappedType = this.Method.ReturnType.ToDisplayString();
                if (firstType != mappedType) throw new InvalidOperationException("Incompatible folded types");
                return firstType;
            }
        }
    }
}
