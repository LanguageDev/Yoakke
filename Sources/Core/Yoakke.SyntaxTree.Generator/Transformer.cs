// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// Describes a visitor.
    /// </summary>
    internal class Transformer
    {
        /// <summary>
        /// The node that defined or overrode this transformer.
        /// </summary>
        public MetaNode Owner { get; }

        /// <summary>
        /// The name of this transformer.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type the transformer produces.
        /// </summary>
        public INamedTypeSymbol TargetType { get; }

        /// <summary>
        /// The source code of this transformer.
        /// </summary>
        public StringBuilder Code { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Transformer"/> class.
        /// </summary>
        /// <param name="owner">The owner <see cref="MetaNode"/>.</param>
        /// <param name="name">The name of this transformer.</param>
        /// <param name="targetType">The target type the transformer produces.</param>
        public Transformer(MetaNode owner, string name, INamedTypeSymbol targetType)
        {
            this.Owner = owner;
            this.Name = name;
            this.TargetType = targetType;
        }
    }
}
