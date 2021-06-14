// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// A single, colored token in the source code.
    /// </summary>
    public readonly struct ColoredToken
    {
        /// <summary>
        /// The start index of the token.
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// The length of the token.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The kind of the token.
        /// </summary>
        public readonly TokenKind Kind;

        public ColoredToken(int start, int length, TokenKind kind)
        {
            this.Start = start;
            this.Length = length;
            this.Kind = kind;
        }
    }
}
