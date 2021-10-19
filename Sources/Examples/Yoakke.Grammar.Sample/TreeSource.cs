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
    public interface ITreeSource
    {
        public bool IsEnd { get; }

        public IIncrementalTreeNode Next(int? currentState);

        public void Reset(IIncrementalTreeNode? node);

        public void MakeEdit(int start, int eraseLength, IEnumerable<Terminal> insertions);
    }

    public class TerminalTreeSource : ITreeSource
    {
        public bool IsEnd => this.index >= this.terminals.Count;

        private readonly List<Terminal> terminals;
        private int index;

        public TerminalTreeSource(IEnumerable<Terminal> terminals)
        {
            this.terminals = terminals.ToList();
        }

        public void Reset(IIncrementalTreeNode? node) => this.index = 0;

        public IIncrementalTreeNode Next(int? currentState) =>
            new LeafIncrementalTreeNode(this.terminals[this.index++], currentState ?? -1);

        public void MakeEdit(int start, int eraseLength, IEnumerable<Terminal> insertions)
        {
            this.terminals.RemoveRange(start, eraseLength);
            this.terminals.InsertRange(start, insertions);
        }
    }

    public class IncrementalTreeSource : ITreeSource
    {
        public bool IsEnd => this.index >= this.nodes.Count;

        private readonly List<IIncrementalTreeNode> nodes;
        private int index;

        public IncrementalTreeSource(IEnumerable<Terminal> terminals)
        {
            this.nodes = terminals.Select(t => (IIncrementalTreeNode)new LeafIncrementalTreeNode(t, -1)).ToList();
        }

        public void Reset(IIncrementalTreeNode? node)
        {
            this.index = 0;
            if (node is not null)
            {
                this.nodes.Clear();
                this.nodes.Add(node);
            }
        }

        public IIncrementalTreeNode Next(int? currentState)
        {
            // Initial terminal
            if (this.nodes[this.index].ParserState == -1) return this.nodes[this.index++];

            // TODO
            throw new NotImplementedException();
        }

        public void MakeEdit(int start, int eraseLength, IEnumerable<Terminal> insertions)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
