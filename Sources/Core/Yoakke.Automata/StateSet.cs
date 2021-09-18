// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a set of states that result from transformations merging multiple states.
    /// Useful for keeping readability.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed class StateSet<TState> : IReadOnlyCollection<TState>, IEquatable<StateSet<TState>>
    {
        /// <inheritdoc/>
        public int Count => this.states.Count;

        private readonly HashSet<TState> states;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
        /// </summary>
        /// <param name="states">The states this set consists of.</param>
        /// <param name="equalityComparer">The comparer to use.</param>
        public StateSet(IEnumerable<TState> states, IEqualityComparer<TState> equalityComparer)
        {
            this.states = new(states, equalityComparer);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is StateSet<TState> other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(StateSet<TState> other)
        {
            if (!this.states.Comparer.Equals(other.states.Comparer)) throw new InvalidOperationException("The comparer of the two state set are different");
            return this.states.SetEquals(other.states);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var h = 0;
            // NOTE: We use XOR to have order-independent hashing
            foreach (var s in this.states) h ^= this.states.Comparer.GetHashCode(s);
            return h;
        }

        /// <inheritdoc/>
        public override string ToString() => string.Join(", ", this.states);

        /// <inheritdoc/>
        public IEnumerator<TState> GetEnumerator() => this.states.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.states as IEnumerable).GetEnumerator();
    }
}
