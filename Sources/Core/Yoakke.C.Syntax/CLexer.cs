// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A lexer that lexes C source tokens, including preprocessor directives.
    /// </summary>
    public class CLexer : LexerBase<CToken>
    {
        /// <summary>
        /// The logical <see cref="Position"/> in the source.
        /// This takes line continuations as being in the same line as the previous one for example.
        /// </summary>
        public Position LogicalPosition { get; private set; }

        /// <summary>
        /// True, if line continuations should be enabled with '\'.
        /// </summary>
        public bool AllowLineContinuations { get; set; } = true;

        /// <summary>
        /// True, if digraphs should be enabled.
        /// </summary>
        public bool AllowDigraphs { get; set; } = true;

        /// <summary>
        /// True, if trigraphs should be enabled.
        /// </summary>
        public bool AllowTrigraphs { get; set; } = true;

        // Our escaped character peek-buffer
        private readonly RingBuffer<char> escapedPeek = new();
        // Our escaped character textual positions
        // We are always a bit ahead in the primitive peek buffer because we always consume everything ASAP
        // This way the physical position gets de-synced from what we are lexing
        // To work around this, we simply store the physical positions of the peeked elements
        private readonly RingBuffer<Position> escapedPosition = new();

        private char prevChar;

        public CLexer(TextReader reader)
            : base(reader)
        {
        }

        public CLexer(string source)
            : base(source)
        {
        }

        public override CToken Next() => throw new NotImplementedException();
    }
}
