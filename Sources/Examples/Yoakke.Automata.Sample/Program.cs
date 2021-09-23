using System;
using Yoakke.Automata.Sparse;

namespace Yoakke.Automata.Sample
{
    internal class Program
    {
        /*
         TODO: what we could do still
          - Improve the StateSet interface (see the note there)
          - Implement dense automata representations
          - Make the minimization algo take an IStateDifferentiator or something instead of state pairs
          - Implement epsilon elimination
         */

        internal static void Main(string[] args)
        {
            var nfa = new Nfa<string, char>();
            nfa.InitialStates.Add("q0");
            nfa.AcceptingStates.Add("q2");
            nfa.AddTransition("q0", '1', "q1");
            nfa.AddTransition("q1", '1', "q0");
            nfa.AddEpsilonTransition("q0", "q2");
            nfa.AddTransition("q2", '0', "q3");
            nfa.AddTransition("q3", '0', "q2");
            nfa.AddTransition("q2", '1', "q4");
            nfa.AddTransition("q4", '0', "q2");

            Console.WriteLine(nfa.ToDot());

            nfa.EliminateEpsilonTransitions();
            Console.WriteLine(nfa.ToDot());
        }
    }
}
