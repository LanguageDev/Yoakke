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
        /// The source code of this visitor.
        /// </summary>
        public StringBuilder Code { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Visitor"/> class.
        /// </summary>
        /// <param name="owner">The owner <see cref="MetaNode"/>.</param>
        /// <param name="name">The name of this visitor.</param>
        /// <param name="returnType">The return type the visitor produces.</param>
        public Visitor(MetaNode owner, string name, INamedTypeSymbol returnType)
        {
            this.Owner = owner;
            this.Name = name;
            this.ReturnType = returnType;
        }
    }
}
