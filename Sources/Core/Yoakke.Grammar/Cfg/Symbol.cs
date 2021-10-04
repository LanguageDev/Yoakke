// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents some symbol in the production rule of the context-free grammar.
    /// </summary>
    public abstract record Symbol
    {
        /// <summary>
        /// An end-of-input marker.
        /// </summary>
        public static Terminal EndOfInput { get; } = new(new EndOfInputMarker());

        private class EndOfInputMarker
        {
            public override string ToString() => "$";
        }
    }
}
