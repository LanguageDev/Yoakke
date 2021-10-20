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
            new LeafIncrementalTreeNode(this.terminals[this.index++]);

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
            this.nodes = terminals.Select(t => (IIncrementalTreeNode)new LeafIncrementalTreeNode(t)).ToList();
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
            if (currentState is null)
            {
                // The current state is nondeterministic
                // We have to explode until we have a terminal
                for (; this.nodes[this.index].ParserState != -1; this.Explode())
                {
                    // Pass
                }
            }
            else
            {
                // Deterministic current state
                // While the node is not reusable or the states are not equal, we explode
                for (;
                       (!this.nodes[this.index].IsReusable
                    || this.nodes[this.index].ParserState != currentState)
                    && this.nodes[this.index].ParserState != -1; this.Explode())
                {
                    // Pass
                }
            }
            return this.nodes[this.index++];
        }

        public void MakeEdit(int start, int eraseLength, IEnumerable<Terminal> insertions)
        {
            for (var i = 0; i < this.nodes.Count; ++i)
            {
                // TODO
            }
        }

        private void MakeEditImpl(int start, int eraseLength, IIncrementalTreeNode current, Queue<Terminal> insertions)
        {
            // TODO
        }

        private void Explode()
        {
            var node = this.nodes[this.index];
            this.nodes.RemoveAt(this.index);
            this.nodes.InsertRange(this.index, node.Children);
        }
    }
}
