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
                this.nodes.Add(new LeafIncrementalTreeNode(Terminal.EndOfInput));
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
            var offset = 0;
            for (var i = 0; i < this.nodes.Count;)
            {
                var node = this.nodes[i];
                var nodeWidth = node.Width;
                if (Intersects(offset, nodeWidth, start, eraseLength))
                {
                    // The current node is within edit range
                    if (node is LeafIncrementalTreeNode leaf)
                    {
                        // The node is a leaf, which means we need to delete it
                        // But also insert all insertions here
                        this.nodes.RemoveAt(i);
                        this.nodes.InsertRange(i, insertions.Select(t => new LeafIncrementalTreeNode(t)));
                        // Since we inserted everything, we also erase all insertions not to accidentally duplicate them
                        // Also skip the inserted elements, they are not relevant for the edit
                        i += insertions.Count();
                        insertions = Enumerable.Empty<Terminal>();
                    }
                    else
                    {
                        // The node is a production, which means this node is not reusable anymore
                        // and needs pruning in the children
                        var prod = (ProductionIncrementalTreeNode)node;
                        this.MakeEditImpl(offset, start, eraseLength, prod, insertions);
                        ++i;
                    }
                }
                else
                {
                    // Not within edit range, just skip node
                    ++i;
                }
                offset += nodeWidth;
            }
        }

        private bool MakeEditImpl(
            int offset,
            int start,
            int eraseLength,
            ProductionIncrementalTreeNode current,
            IEnumerable<Terminal> insertions)
        {
            current.IsReusable = false;
            var insertedTerminals = false;
            var i = 0;
            var origOffset = offset;
            if (offset == start)
            {
                // The offset is just at the start of the edit, insert the terminals
                foreach (var t in insertions) current.Children.Insert(i++, new LeafIncrementalTreeNode(t));
                insertions = Enumerable.Empty<Terminal>();
            }
            // Deal with the remaining children
            for (; i < current.Children.Count;)
            {
                var node = current.Children[i];
                var nodeWidth = node.Width;
                if (Intersects(offset, nodeWidth, start, eraseLength))
                {
                    // The current node is within edit range
                    if (node is LeafIncrementalTreeNode)
                    {
                        // The node is a leaf, which means we need to delete it
                        // But also insert all insertions here
                        current.Children.RemoveAt(i);
                    }
                    else
                    {
                        // The node is a production, which means this node is not reusable anymore
                        // and needs pruning in the children
                        var prod = (ProductionIncrementalTreeNode)node;
                        insertedTerminals = this.MakeEditImpl(offset, start, eraseLength, prod, insertions) || insertedTerminals;
                        ++i;
                    }
                }
                else
                {
                    // Not within edit range, just skip node
                    ++i;
                }
                offset += nodeWidth;
            }

            if (!insertedTerminals)
            {
                offset = origOffset;
                for (var j = 0; j < current.Children.Count; ++j)
                {
                    if (offset <= start && start < offset + current.Children[j].Width)
                    {
                        foreach (var n in insertions) current.Children.Insert(j++, new LeafIncrementalTreeNode(n));
                        insertedTerminals = true;
                        break;
                    }
                    offset += current.Children[j].Width;
                }
            }

            return insertedTerminals;
        }

        private void Explode()
        {
            var node = this.nodes[this.index];
            this.nodes.RemoveAt(this.index);
            this.nodes.InsertRange(this.index, node.Children);
        }

        private static bool Intersects(int start1, int len1, int start2, int len2)
        {
            var end1 = start1 + len1;
            var end2 = start2 + len2;
            return !(end1 <= start2 || end2 <= start1);
        }
    }
}
