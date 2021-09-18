// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// An equality comparer for <see cref="StateSet{TState}"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of states.</typeparam>
    public class StateSetEqualityComparer<TState> : IEqualityComparer<StateSet<TState>>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static StateSetEqualityComparer<TState> Default { get; } = new(EqualityComparer<TState>.Default);

        /// <summary>
        /// The state comparer to use.
        /// </summary>
        public IEqualityComparer<TState> Comparer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSetEqualityComparer{TState}"/> class.
        /// </summary>
        /// <param name="comparer">The state comparer to use.</param>
        public StateSetEqualityComparer(IEqualityComparer<TState> comparer)
        {
            this.Comparer = comparer;
        }

        /// <inheritdoc/>
        public bool Equals(StateSet<TState> x, StateSet<TState> y) =>
               x.Count == y.Count
            && x.Intersect(y, this.Comparer).Count() == x.Count;

        /// <inheritdoc/>
        public int GetHashCode(StateSet<TState> obj)
        {
            var h = 0;
            // NOTE: We use XOR to have order-independent hashing
            foreach (var s in obj) h ^= this.Comparer.GetHashCode(s);
            return h;
        }
    }
}
