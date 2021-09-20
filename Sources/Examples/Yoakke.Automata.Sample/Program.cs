using System;
using Yoakke.Automata.Sparse;

namespace Yoakke.Automata.Sample
{
    class Program
    {
        /*
         TODO: what we could do still
          - Improve the StateSet interface (see the note there)
          - The I(ReadOnly)SparseAutomata interfaces could get ISparseNfa and Dfa pairs
            (these are just essentially convenience interfaces)
            IReadOnlySparseDfa, ISparseDfa, IReadOnlySparseNfa, ISparseNfa
          - Move initial state to the NFA and DFA interfaces, to allow NFA to have multiple initial states
          - Implement the sparse automata interface for the NFA too
          - Implement dense automata representations
          - Move symbol comparer to be required in the interfaces too
         */

        static void Main(string[] args)
        {
            var nfa = new Nfa<string, char>();
            nfa.InitialState = "A";
            nfa.AcceptingStates.Add("D");
            nfa.AddTransition("A", '0', "A");
            nfa.AddTransition("A", '1', "A");
            nfa.AddTransition("A", '1', "B");
            nfa.AddTransition("B", '0', "C");
            nfa.AddEpsilonTransition("B", "C");
            nfa.AddTransition("C", '1', "D");
            nfa.AddTransition("D", '0', "D");
            nfa.AddTransition("D", '1', "D");

            var dfa = nfa.Determinize();
            var minDfa = dfa.Minimize(StateCombiner<string>.DefaultSetCombiner);

            Console.WriteLine(nfa.ToDot());
            Console.WriteLine(dfa.ToDot());
            Console.WriteLine(minDfa.ToDot());
        }
    }
}
