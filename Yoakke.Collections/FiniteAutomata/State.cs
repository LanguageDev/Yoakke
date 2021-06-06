using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// Represents a single state of a finite automata.
    /// </summary>
    public sealed class State : IEquatable<State>, IComparable<State>
    {
        public static readonly State Invalid = new State(Enumerable.Empty<State>());

        private readonly int[] indices;

        public State(params int[] index)
        {
            indices = index;
        }

        public State(IEnumerable<State> parentStates)
        {
            indices = parentStates.SelectMany(s => s.indices).Distinct().OrderBy(i => i).ToArray();
        }

        public override bool Equals(object obj) => obj is State s && Equals(s);
        public bool Equals(State other) => indices.SequenceEqual(other.indices);
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var e in indices) hash.Add(e);
            return hash.ToHashCode();
        }

        public int CompareTo(State other)
        {
            for (int i = 0; i < Math.Min(indices.Length, other.indices.Length); ++i)
            {
                var diff = indices[i] - other.indices[i];
                if (diff != 0) return diff;
            }
            var lenDiff = indices.Length - other.indices.Length;
            if (lenDiff != 0) return lenDiff;
            return 0;
        }

        public override string ToString() => indices.Length == 0 ? "INVALID STATE" : $"q{string.Join("_", indices)}";

        public static bool operator ==(State s1, State s2) => s1.Equals(s2);
        public static bool operator !=(State s1, State s2) => !s1.Equals(s2);

        public bool IsSubstateOf(State other) => indices.All(i => other.indices.Contains(i));
    }
}
