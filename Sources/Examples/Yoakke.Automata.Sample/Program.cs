using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Automata.Dense;
using Yoakke.Automata.Sparse;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Sample
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var nfa = new DenseNfa<string, char>();
            nfa.InitialStates.Add("A");
            nfa.AcceptingStates.Add("E");
            nfa.AcceptingStates.Add("F");

            void AddTransition(string from, string on, string to) =>
                nfa!.AddTransition(from, Interval<char>.Parse(on, t => t[0]), to);

            StateSet<string> ToStateSet(string s) => new(s.Split(",").Select(s => s.Trim()), EqualityComparer<string>.Default);

            nfa.AddEpsilonTransition("A", "B");
            nfa.AddEpsilonTransition("A", "C");
            AddTransition("B", "[i; i]", "D");
            AddTransition("D", "[f; f]", "F");
            AddTransition("C", "[a; z]", "E");
            AddTransition("E", "[a; z]", "E");

            var dfa = nfa.Determinize();
            var minDfa = dfa.Minimize(StateCombiner<string>.DefaultSetCombiner, new[] { (ToStateSet("E, F"), ToStateSet("E")) });

            Console.WriteLine(nfa.ToDot());
            Console.WriteLine(dfa.ToDot());
            Console.WriteLine(minDfa.ToDot());
        }
    }
}
