// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a general lexer.
    /// It's a stateful iterator-like object that reads in <see cref="IToken{TKind}"/>s from a text source.
    /// </summary>
    /// <typeparam name="TKind">The token-type of the read in <see cref="IToken{TKind}"/>s.</typeparam>
    public interface ILexer<TKind> : ILexer
    {
        /// <summary>
        /// Lexes the next <see cref="IToken{TKind}"/>. If the source text has been depleted, it should produce some default
        /// end-signaling <see cref="IToken{TKind}"/>.
        /// </summary>
        /// <returns>The lexed <see cref="IToken{TKind}"/>.</returns>
        public new IToken<TKind> Next();
    }
}
