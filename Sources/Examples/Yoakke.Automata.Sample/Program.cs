using System;
using Yoakke.Automata.Discrete;

namespace Yoakke.Automata.Sample
{
    class Program
    {
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
