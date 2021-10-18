// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Sample
{
    abstract class Vertex
    {
        public abstract IEnumerable<Vertex> Prev { get; }
    }

    class StateVertex : Vertex
    {
        public int State { get; }

        public override IEnumerable<Vertex> Prev => this.PrevMap.Values;

        public Dictionary<Symbol, SymbolVertex> PrevMap { get; } = new Dictionary<Symbol, SymbolVertex>();

        public StateVertex()
        {
        }

        public StateVertex(SymbolVertex prev, int state)
        {
            this.PrevMap.Add(prev.Symbol, prev);
            this.State = state;
        }
    }

    class SymbolVertex : Vertex
    {
        public Symbol Symbol { get; }

        public override ISet<StateVertex> Prev { get; } = new HashSet<StateVertex>();

        public SymbolVertex(StateVertex prev, Symbol symbol)
        {
            this.Prev.Add(prev);
            this.Symbol = symbol;
        }
    }
}
