using System;
using Yoakke.Automata.Discrete;

namespace Yoakke.Automata.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dfa = new Dfa<string, char>();

            dfa.AddTransition("S", 'a', "A");
            dfa.AddTransition("S", 'b', "B");
            dfa.AddTransition("B", 'b', "BB");
            dfa.AddTransition("BB", 'b', "BB");
            dfa.AddTransition("BB", 'a', "BA");
            dfa.AddTransition("B", 'a', "BA");
            dfa.AddTransition("A", 'a', "AA");
            dfa.AddTransition("A", 'b', "AB");
            dfa.AddTransition("AA", 'a', "AA");
            dfa.AddTransition("AA", 'b', "AB");
            dfa.AddTransition("AB", 'a', "BA");
            dfa.AddTransition("AB", 'b', "BB");
            dfa.AddTransition("BA", 'a', "AA");
            dfa.AddTransition("BA", 'b', "AB");

            dfa.AcceptingStates.Add("AB");
            dfa.AcceptingStates.Add("BA");

            dfa.InitialState = "S";

            Console.WriteLine(dfa.ToDot());
        }
    }
}
