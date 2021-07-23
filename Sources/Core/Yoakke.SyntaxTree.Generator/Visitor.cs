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
    internal class Visitor
    {
        /// <summary>
        /// The node that defined or overrode this visitor.
        /// </summary>
        public MetaNode Owner { get; }

        /// <summary>
        /// The name of this visitor.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type the visitor returns.
        /// </summary>
        public INamedTypeSymbol ReturnType { get; }

        /// <summary>
        /// True, if this visitor is actually a transformer.
        /// </summary>
        public bool IsTransformer { get; }

        /// <summary>
        /// The source code of this visitor.
        /// </summary>
        public StringBuilder Code { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Visitor"/> class.
        /// </summary>
        /// <param name="owner">The owner <see cref="MetaNode"/>.</param>
        /// <param name="name">The name of this visitor.</param>
        /// <param name="returnType">The return type the visitor produces.</param>
        /// <param name="isTransformer">True, if the visitor is a transformer.</param>
        public Visitor(MetaNode owner, string name, INamedTypeSymbol returnType, bool isTransformer = false)
        {
            this.Owner = owner;
            this.Name = name;
            this.ReturnType = returnType;
            this.IsTransformer = isTransformer;
        }
    }
}
