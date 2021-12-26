// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator;

/// <summary>
/// A simple structure describing overrides for a section of the tree visitor.
/// </summary>
internal class VisitorOverride
{
    /// <summary>
    /// The node class this override applies to.
    /// </summary>
    public INamedTypeSymbol NodeClass { get; }

    /// <summary>
    /// Override for the method name.
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Override for the method return type.
    /// Can be either a <see cref="INamedTypeSymbol"/>, or a generic parameter name as a <see cref="string"/>.
    /// </summary>
    public object? ReturnType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisitorOverride"/> class.
    /// </summary>
    /// <param name="nodeClass">The node class this override applies to.</param>
    public VisitorOverride(INamedTypeSymbol nodeClass)
    {
        this.NodeClass = nodeClass;
    }
}
