// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a stream of <see cref="IToken"/>s that can be read and (optionally) written sequentially.
    /// </summary>
    public interface ITokenStream : ITokenStream<IToken>
    {
    }
}
