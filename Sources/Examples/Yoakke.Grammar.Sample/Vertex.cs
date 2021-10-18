// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.ParseTree;

namespace Yoakke.Grammar.Sample
{
    public abstract class Vertex
    {
        public abstract IEnumerable<Vertex> Prev { get; }
    }

    public class StateVertex : Vertex
    {
        public int State { get; init; }

        public override IEnumerable<Vertex> Prev => this.PrevMap.Values;

        public Dictionary<IParseTreeNode, SymbolVertex> PrevMap { get; init; } = new Dictionary<IParseTreeNode, SymbolVertex>();

        public StateVertex()
        {
        }

        public StateVertex(SymbolVertex prev, int state)
        {
            this.PrevMap.Add(prev.ParseTree, prev);
            this.State = state;
        }

        public StateVertex Clone() => new()
        {
            State = this.State,
            PrevMap = this.PrevMap.ToDictionary(kv => kv.Key, kv => kv.Value.Clone()),
        };
    }

    public class SymbolVertex : Vertex
    {
        public Symbol Symbol => this.ParseTree.Symbol;

        public IParseTreeNode ParseTree { get; }

        public override ISet<StateVertex> Prev { get; } = new HashSet<StateVertex>();

        private SymbolVertex(IParseTreeNode treeNode)
        {
            this.ParseTree = treeNode;
        }

        public SymbolVertex(StateVertex prev, IParseTreeNode treeNode)
        {
            this.Prev.Add(prev);
            this.ParseTree = treeNode;
        }

        public SymbolVertex Clone()
        {
            var result = new SymbolVertex(this.ParseTree);
            foreach (var p in this.Prev) result.Prev.Add(p.Clone());
            return result;
        }
    }
}
