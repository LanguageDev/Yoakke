// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Text;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a general lexer.
    /// It's a stateful iterator-like object that reads in <see cref="IToken"/>s from a text source.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// The current <see cref="Text.Position"/> the <see cref="ILexer"/> is at in the source.
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// True, if all of the input has been consumed.
        /// </summary>
        public bool IsEnd { get; }

        /// <summary>
        /// Lexes the next <see cref="IToken"/>. If the source text has been depleted, it should produce some default
        /// end-asignaling <see cref="IToken"/>.
        /// </summary>
        /// <returns>The lexed <see cref="IToken"/>.</returns>
        public IToken Next();
    }
}
