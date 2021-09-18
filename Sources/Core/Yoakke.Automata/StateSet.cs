// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IReadOnlyList<TState> states;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
        /// </summary>
        /// <param name="states">The states this set consists of.</param>
        public StateSet(IEnumerable<TState> states)
        {
            this.states = states.ToList();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is StateSet<TState> other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(StateSet<TState> other) => StateSetEqualityComparer<TState>.Default.Equals(this, other);

        /// <inheritdoc/>
        public override int GetHashCode() => StateSetEqualityComparer<TState>.Default.GetHashCode(this);

        /// <inheritdoc/>
        public override string ToString() => string.Join(", ", this.states);

        /// <inheritdoc/>
        public IEnumerator<TState> GetEnumerator() => this.states.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.states as IEnumerable).GetEnumerator();
    }
}
