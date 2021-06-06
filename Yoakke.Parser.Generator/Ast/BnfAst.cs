using System;

namespace Yoakke.Parser.Generator.Ast
{
    /// <summary>
    /// Base-class for the grammar syntax-tree nodes.
    /// </summary>
    internal abstract partial class BnfAst : IEquatable<BnfAst>
    {
        public override bool Equals(object obj) => obj is BnfAst bnf && Equals(bnf);

        public abstract bool Equals(BnfAst other);
        public abstract override int GetHashCode();

        /// <summary>
        /// Desugars the AST into simpler elements.
        /// </summary>
        /// <returns>The desugared <see cref="BnfAst"/>.</returns>
        public abstract BnfAst Desugar();

        /// <summary>
        /// Calculates what the result type would be, if parsing the thing this AST describes.
        /// </summary>
        /// <param name="ruleSet">The set of available rule definitions.</param>
        /// <param name="tokens">The set of available token-types.</param>
        /// <returns>The string that describes the parsed type.</returns>
        public abstract string GetParsedType(RuleSet ruleSet, TokenKindSet tokens);
    }
}
