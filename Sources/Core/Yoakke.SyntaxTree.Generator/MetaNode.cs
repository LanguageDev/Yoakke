// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// A meta-AST node.
    /// Used as the data-structure for the AST source generator, to describe the AST hierarchy.
    /// </summary>
    internal class MetaNode
    {
        /// <summary>
        /// The corresponding AST node symbol.
        /// </summary>
        public INamedTypeSymbol Symbol { get; }

        /// <summary>
        /// The parent <see cref="MetaNode"/> of this one.
        /// This corresponds to the fact, that <see cref="Symbol"/> inherits from the <see cref="Parent"/>s symbol.
        /// </summary>
        public MetaNode? Parent { get; }

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string Name => this.Symbol.Name;

        /// <summary>
        /// True, if this node is abstract.
        /// </summary>
        public bool IsAbstract => this.Symbol.IsAbstract;

        /// <summary>
        /// The list of identifiers this node is nested in.
        /// </summary>
        public IList<string> Nesting { get; }

        /// <summary>
        /// The nodes that inherit from this one.
        /// </summary>
        public IDictionary<string, MetaNode> Children { get; } = new Dictionary<string, MetaNode>();

        /// <summary>
        /// The visitors defined for this node.
        /// </summary>
        public IList<(string Name, Type ReturnType)> Visitors { get; } = new List<(string Name, Type ReturnType)>();

        /// <summary>
        /// The root node, so the ancestor with no parent.
        /// </summary>
        public MetaNode Root => this.Parent == null ? this : this.Parent.Root;

        private bool? implementEquality;

        /// <summary>
        /// True, if equality and hash should be implemented.
        /// </summary>
        public bool ImplementEquality
        {
            get => this.implementEquality ?? this.Parent?.ImplementEquality ?? false;
            set => this.implementEquality = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaNode"/> class.
        /// </summary>
        /// <param name="symbol">The corresponding type symbol.</param>
        /// <param name="parent">The parent node of this one.</param>
        public MetaNode(INamedTypeSymbol symbol, MetaNode? parent)
        {
            this.Symbol = symbol;
            this.Parent = parent;
            this.Nesting = GetNesting(this.Symbol);
        }

        private static IList<string> GetNesting(INamedTypeSymbol symbol)
        {
            var result = new List<string>();
            for (var parent = symbol.ContainingType; parent != null; parent = parent.ContainingType)
            {
                result.Insert(0, parent.Name);
            }
            return result;
        }
    }
}
