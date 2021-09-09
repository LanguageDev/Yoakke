// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.LexerUtils.FiniteAutomata;

namespace Yoakke.LexerUtils.RegEx
{
    /// <summary>
    /// The base class for all regular expression AST nodes.
    /// </summary>
    public abstract partial class RegExAst : IEquatable<RegExAst>
    {
        /// <summary>
        /// Desugars this node into simpler regex constructs.
        /// </summary>
        /// <returns>The desugared node.</returns>
        public abstract RegExAst Desugar();

        /// <summary>
        /// Thompson constructs this regex node into a dense NFA.
        /// </summary>
        /// <param name="denseNfa">The dense NFA to do the construction inside.</param>
        /// <returns>The starting and ending state of the NFA construct.</returns>
        public abstract (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RegExAst r && this.Equals(r);

        /// <inheritdoc/>
        public abstract bool Equals(RegExAst other);

        /// <inheritdoc/>
        public override abstract int GetHashCode();
    }
}
