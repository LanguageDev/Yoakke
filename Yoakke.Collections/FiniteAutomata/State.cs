using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// Represents a single state of a finite automata.
    /// </summary>
    public struct State : IEquatable<State>
    {
        public static State Invalid = new State(-1);

        private int index;

        public State(int index)
        {
            this.index = index;
        }

        public override bool Equals(object obj) => obj is State s && Equals(s);
        public bool Equals(State other) => index == other.index;
        public override int GetHashCode() => index.GetHashCode();

        public override string ToString() => index == -1 ? "INVALID STATE" : $"q{index}";

        public static bool operator ==(State s1, State s2) => s1.Equals(s2);
        public static bool operator !=(State s1, State s2) => !s1.Equals(s2);
    }
}
