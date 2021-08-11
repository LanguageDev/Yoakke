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
    /// Description of a single visitor.
    /// </summary>
    internal class Visitor
    {
        /// <summary>
        /// The visitor class that is annotated.
        /// </summary>
        public INamedTypeSymbol VisitorClass { get; }

        /// <summary>
        /// A node class to return-type override map.
        /// </summary>
        // NOTE: False-positive
#pragma warning disable RS1024 // Compare symbols correctly
        public IDictionary<INamedTypeSymbol, VisitorOverride> Overrides { get; }
            = new Dictionary<INamedTypeSymbol, VisitorOverride>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

        /// <summary>
        /// Initializes a new instance of the <see cref="Visitor"/> class.
        /// </summary>
        /// <param name="visitorClass">The visitor class that is annotated.</param>
        public Visitor(INamedTypeSymbol visitorClass)
        {
            this.VisitorClass = visitorClass;
        }
    }
}
