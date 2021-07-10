// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Utilities.FiniteAutomata
{
    /// <summary>
    /// Represents a single state of a finite automata.
    /// </summary>
    public sealed class State : IEquatable<State>, IComparable<State>
    {
        public static readonly State Invalid = new(Enumerable.Empty<State>());

        private readonly int[] indices;

        public State(params int[] index)
        {
            this.indices = index;
        }

        public State(IEnumerable<State> parentStates)
        {
            this.indices = parentStates.SelectMany(s => s.indices).Distinct().OrderBy(i => i).ToArray();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is State s && this.Equals(s);

        /// <inheritdoc/>
        public bool Equals(State other) => this.indices.SequenceEqual(other.indices);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = default(HashCode);
            foreach (var e in this.indices) hash.Add(e);
            return hash.ToHashCode();
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

        public static bool operator ==(State s1, State s2) => s1.Equals(s2);

        public static bool operator !=(State s1, State s2) => !s1.Equals(s2);

        public bool IsSubstateOf(State other) => this.indices.All(i => other.indices.Contains(i));
    }
}
