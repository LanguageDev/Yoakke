// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Parser.Generator.Ast
{
    /// <summary>
    /// Base-class for the grammar syntax-tree nodes.
    /// </summary>
    internal abstract partial class BnfAst
    {
        /// <summary>
        /// Desugars the AST into simpler elements.
        ///
        /// The order of elements from top level to lower levels is Alt, Transform, Seq and finally everything else.
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
