// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// Extensions for <see cref="ICharStream"/>.
    /// </summary>
    public static class CharStreamExtensions
    {
        /// <summary>
        /// Peeks ahead some characters into the input.
        /// </summary>
        /// <param name="stream">The stream to peek.</param>
        /// <param name="offset">The amount to peek forward. 0 means next character.</param>
        /// <param name="default">The default character to return if the end has been reached.</param>
        /// <returns>The peeked character, or default if the end has been reached.</returns>
        public static char LookAhead(ICharStream stream, int offset = 0, char @default = '\0') =>
            stream.TryLookAhead(offset, out var ch) ? ch : @default;
    }
}
