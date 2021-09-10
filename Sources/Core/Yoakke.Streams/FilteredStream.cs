// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.Streams
{
    /// <summary>
    /// A filtered <see cref="IStream{T}"/> with a predicate.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class FilteredStream<T> : IStream<T>
    {
        /// <inheritdoc/>
        public bool IsEnd => this.Underlying.IsEnd;

        /// <summary>
        /// The underlying stream this one reads from.
        /// </summary>
        public IStream<T> Underlying { get; }

        /// <summary>
        /// The predicate used for filtering.
        /// </summary>
        public Predicate<T> Predicate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredStream{T}"/> class.
        /// </summary>
        /// <param name="underlying">The underlying stream this one reads from.</param>
        /// <param name="predicate">The predicate used for filtering.</param>
        public FilteredStream(IStream<T> underlying, Predicate<T> predicate)
        {
            this.Underlying = underlying;
            this.Predicate = predicate;
        }

        /// <inheritdoc/>
        public bool TryConsume([MaybeNullWhen(false)] out T item)
        {
            while (true)
            {
                if (!this.Underlying.TryConsume(out item)) return false;
                if (!this.Predicate(item)) continue;
                return true;
            }
        }

        /// <inheritdoc/>
        public int Consume(int amount) => StreamExtensions.Consume(this, amount);
    }
}
