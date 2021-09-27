using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Automata.Dense;
using Yoakke.Automata.RegExAst;
using Yoakke.Automata.Sparse;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Sample
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            // Construct a regex
            var regex = RegEx.Seq(RegEx.Or(RegEx.Lit('H'), RegEx.Lit('h')), RegEx.Lit('i'));
            regex = regex.Desugar();

            int stateCount = 0;
            int MakeState() => stateCount++;

            var nfa = new DenseNfa<int, char>();
            var (start, end) = regex.ThompsonsConstruct(nfa, MakeState);
            nfa.InitialStates.Add(start);
            nfa.AcceptingStates.Add(end);

            var dfa = nfa.Determinize();
            var minDfa = dfa.Minimize(StateCombiner<int>.DefaultSetCombiner);

            Console.WriteLine(nfa.ToDot());
            nfa.EliminateEpsilonTransitions();
            Console.WriteLine(nfa.ToDot());
            nfa.RemoveUnreachable();
            Console.WriteLine(nfa.ToDot());
            Console.WriteLine(dfa.ToDot());
            Console.WriteLine(minDfa.ToDot());
        }
    }
}
