using System;
using Yoakke.Automata.Dense;
using Yoakke.Automata.Sparse;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Sample
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var dfa = new DenseDfa<string, char>();
            dfa.InitialState = "A";
            dfa.AcceptingStates.Add("F");
            dfa.AcceptingStates.Add("G");

            void AddTransition(string from, string on, string to) =>
                dfa!.AddTransition(from, Interval<char>.Parse(on, t => t[0]), to);

            AddTransition("A", "[a; z]", "B");
            AddTransition("A", "[0; 9]", "C");
            AddTransition("B", "[a; f]", "D");
            AddTransition("C", "[a; f]", "D");
            AddTransition("B", "[0; 5]", "E");
            AddTransition("C", "[0; 5]", "E");
            AddTransition("D", "[A; J]", "F");
            AddTransition("E", "[A; L)", "G");

            var minDfa = dfa.Minimize();

            Console.WriteLine(minDfa.ToDot());
        }
    }
}
