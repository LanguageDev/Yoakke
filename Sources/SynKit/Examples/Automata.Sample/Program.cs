// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.SynKit.Automata.RegExAst;
using Yoakke.SynKit.Automata.Sparse;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Sample;

internal class Program
{
    internal static void Main(string[] args)
    {
        var dfa = new DenseDfa<int, char>();
        dfa.InitialState = 0;
        dfa.AcceptingStates.Add(3);
        dfa.AcceptingStates.Add(4);
        dfa.AddTransition(0, '0', 1);
        dfa.AddTransition(1, 'x', 2);
        dfa.AddTransition(2, '0', 3);
        dfa.AddTransition(3, '0', 4);
        dfa.AddTransition(4, '0', 4);

        var minDfa = dfa.Minimize();

        Console.WriteLine(dfa.ToDot());
        Console.WriteLine(minDfa.ToDot());
    }
}
