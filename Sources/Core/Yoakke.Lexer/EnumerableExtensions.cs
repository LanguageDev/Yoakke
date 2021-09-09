// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Lexer.Streams;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}"/>s.
    /// </summary>
    public static class EnumerableExtensions
    {
        private class EnumerableAdapter<TToken> : ITokenStream<TToken>
            where TToken : IToken
        {
            public bool IsEnd { get; private set; }

            private readonly IEnumerator<TToken> enumerator;

            public EnumerableAdapter(IEnumerable<TToken> items)
            {
                this.enumerator = items.GetEnumerator();
                this.IsEnd = this.enumerator.MoveNext();
            }

            public bool TryAdvance([MaybeNullWhen(false)] out TToken token)
            {
                if (this.IsEnd)
                {
                    token = default;
                    return false;
                }
                else
                {
                    token = this.enumerator.Current;
                    this.IsEnd = this.enumerator.MoveNext();
                    return true;
                }
            }

            public int Advance(int amount) => throw new NotSupportedException();

            public void Defer(TToken token) => throw new NotSupportedException();

            public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out TToken token) => throw new NotSupportedException();

            public bool TryPeek([MaybeNullWhen(false)] out TToken token) => throw new NotSupportedException();
        }

        /// <summary>
        /// Adapts an <see cref="IEnumerable{TToken}"/> to be an <see cref="ITokenStream{TToken}"/>.
        /// </summary>
        /// <typeparam name="TToken">The exact <see cref="IToken"/> contained by the enumerable.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{TToken}"/> to adapt.</param>
        /// <returns>An <see cref="ITokenStream{TToken}"/> that supports all operations and reads from
        /// <paramref name="items"/>.</returns>
        public static ITokenStream<TToken> AsTokenStream<TToken>(this IEnumerable<TToken> items)
            where TToken : IToken => new BufferedTokenStream<TToken>(new EnumerableAdapter<TToken>(items));
    }
}
