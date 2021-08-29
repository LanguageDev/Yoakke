// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// A character source for a lexer.
    /// </summary>
    public interface ICharStream
    {
        public char Peek(int offset);
    }
}
