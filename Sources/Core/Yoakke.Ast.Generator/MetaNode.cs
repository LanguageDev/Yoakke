// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.Ast.Generator
{
    internal class MetaNode
    {
        public INamedTypeSymbol Symbol { get; }
        public MetaNode? Parent { get; }

        public string Name => this.Symbol.Name;

        public bool IsAbstract => this.Symbol.IsAbstract;

        public IList<string> Nesting { get; }
        public IDictionary<string, MetaNode> Children { get; } = new Dictionary<string, MetaNode>();
        public IList<(string Name, Type ReturnType)> Visitors { get; } = new List<(string Name, Type ReturnType)>();

        public MetaNode Root => this.Parent == null ? this : this.Parent.Root;

        private bool? implementEquality;

        public bool ImplementEquality
        {
            get => this.implementEquality ?? this.Parent?.ImplementEquality ?? false;
            set => this.implementEquality = value;
        }

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
