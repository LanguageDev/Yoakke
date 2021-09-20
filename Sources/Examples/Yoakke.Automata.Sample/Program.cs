using System;
using Yoakke.Automata.Sparse;

namespace Yoakke.Automata.Sample
{
    class Program
    {
        /*
         TODO: what we could do still
          - Improve the StateSet interface (see the note there)
          - Implement dense automata representations
          - Make the minimization algo take an IStateDifferentiator or something instead of state pairs
         */

        static void Main(string[] args)
        {
            var dfa = new Dfa<string, char>();
            dfa.InitialState = "A";
            dfa.AddTransition("A", 'a', "B");
            dfa.AddTransition("B", 'a', "C");
            dfa.AcceptingStates.Add("C");
            dfa.States.Remove("B");

            Console.WriteLine(dfa.ToDot());
        }
    }
}
