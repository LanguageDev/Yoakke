// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.Utilities.FiniteAutomata
{
    /// <summary>
    /// Represents a single state of a finite automata.
    /// </summary>
    public sealed class State : IEquatable<State>, IComparable<State>
    {
        /// <summary>
        /// Denites an invalid <see cref="State"/>.
        /// </summary>
        public static readonly State Invalid = new(Enumerable.Empty<State>());

        private readonly int[] indices;

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="index">The indicies identifying this <see cref="State"/>.</param>
        public State(params int[] index)
        {
            this.indices = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="parentStates">The <see cref="State"/>s that are the parents of this one.</param>
        public State(IEnumerable<State> parentStates)
        {
            // TODO: Very ineffective statement.
            this.indices = parentStates.SelectMany(s => s.indices).Distinct().OrderBy(i => i).ToArray();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is State s && this.Equals(s);

        /// <inheritdoc/>
        public bool Equals(State other) => this.indices.SequenceEqual(other.indices);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = 0;
            foreach (var e in this.indices) hash = (hash, e).GetHashCode();
            return hash;
        }

        /// <inheritdoc/>
        public int CompareTo(State other)
        {
            for (var i = 0; i < Math.Min(this.indices.Length, other.indices.Length); ++i)
            {
                var diff = this.indices[i] - other.indices[i];
                if (diff != 0) return diff;
            }
            var lenDiff = this.indices.Length - other.indices.Length;
            if (lenDiff != 0) return lenDiff;
            return 0;
        }

        /// <inheritdoc/>
        public override string ToString() => this.indices.Length == 0 ? "INVALID STATE" : $"q{string.Join("_", this.indices)}";

        /// <summary>
        /// Compares two <see cref="State"/>s for equality.
        /// </summary>
        /// <param name="s1">The first <see cref="State"/> to compare.</param>
        /// <param name="s2">The second <see cref="State"/> to compare.</param>
        /// <returns>True, if <paramref name="s1"/> and <paramref name="s2"/> are equal.</returns>
        public static bool operator ==(State s1, State s2) => s1.Equals(s2);

        /// <summary>
        /// Compares two <see cref="State"/>s for inequality.
        /// </summary>
        /// <param name="s1">The first <see cref="State"/> to compare.</param>
        /// <param name="s2">The second <see cref="State"/> to compare.</param>
        /// <returns>True, if <paramref name="s1"/> and <paramref name="s2"/> are not equal.</returns>
        public static bool operator !=(State s1, State s2) => !s1.Equals(s2);

        /// <summary>
        /// Checks, if this <see cref="State"/> is a substate of another.
        /// </summary>
        /// <param name="other">The other <see cref="State"/>.</param>
        /// <returns>True, if this is a substate of <paramref name="other"/>.</returns>
        public bool IsSubstateOf(State other) => this.indices.All(i => other.indices.Contains(i));
    }
}
