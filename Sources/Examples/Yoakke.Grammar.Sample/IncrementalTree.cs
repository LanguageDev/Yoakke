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
    public interface IIncrementalTreeNode : IEquatable<IIncrementalTreeNode>
    {
        public Symbol Symbol { get; }

        public bool IsReusable { get; }

        public int Width { get; }

        public int ParserState { get; }

        public IReadOnlyList<IIncrementalTreeNode> Children { get; }

        public Terminal FirstTerminal { get; }
    }

    public sealed class LeafIncrementalTreeNode : IIncrementalTreeNode
    {
        public Symbol Symbol => this.Terminal;

        public Terminal Terminal { get; }

        public bool IsReusable => true;

        public int Width => 1;

        public int ParserState => -1;

        public IReadOnlyList<IIncrementalTreeNode> Children => Array.Empty<IIncrementalTreeNode>();

        public Terminal FirstTerminal => this.Terminal;

        public LeafIncrementalTreeNode(Terminal terminal)
        {
            this.Terminal = terminal;
        }

        public override bool Equals(object? obj) => this.Equals(obj as IIncrementalTreeNode);

        public bool Equals(IIncrementalTreeNode? other) =>
               other is LeafIncrementalTreeNode leaf
            && this.Terminal.Equals(leaf.Terminal)
            && this.ParserState == leaf.ParserState;

        public override int GetHashCode() => HashCode.Combine(this.Terminal, this.ParserState);
    }

    public sealed class ProductionIncrementalTreeNode : IIncrementalTreeNode
    {
        public Symbol Symbol => this.Production.Left;

        public Production Production { get; }

        public bool IsReusable { get; set; }

        public int Width => this.Children.Sum(c => c.Width);

        public int ParserState { get; }

        IReadOnlyList<IIncrementalTreeNode> IIncrementalTreeNode.Children => this.children;

        public IList<IIncrementalTreeNode> Children => this.children;

        private readonly List<IIncrementalTreeNode> children;

        public Terminal FirstTerminal => this.Children[0].FirstTerminal;

        public ProductionIncrementalTreeNode(Production production, int state, IReadOnlyList<IIncrementalTreeNode> children)
        {
            this.Production = production;
            this.ParserState = state;
            this.children = children.ToList();
        }

        public override bool Equals(object? obj) => this.Equals(obj as IIncrementalTreeNode);

        public bool Equals(IIncrementalTreeNode? other) =>
               other is ProductionIncrementalTreeNode prod
            && this.Production.Equals(prod.Production)
            && this.ParserState == prod.ParserState
            && this.Children.SequenceEqual(prod.Children);

        // NOTE: Not exact, simplification
        public override int GetHashCode() => HashCode.Combine(this.Production, this.Children.Count);
    }
}
