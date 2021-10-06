// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents a terminal symbol.
    /// </summary>
    public sealed record Terminal(object Value) : Symbol
    {
        private class EndOfInputMarker
        {
            public override string ToString() => "$";
        }

        private class NotInGrammarMarker
        {
            public override string ToString() => "#";
        }

        /// <summary>
        /// An end-of-input marker.
        /// </summary>
        public static Terminal EndOfInput { get; } = new(new EndOfInputMarker());

        /// <summary>
        /// A terminal guaranteed to be not in any grammar.
        /// </summary>
        public static Terminal NotInGrammar { get; } = new(new NotInGrammarMarker());

        /// <inheritdoc/>
        public override string ToString() => this.Value.ToString();
    }
}
