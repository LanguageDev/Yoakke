// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.SyntaxTree.Attributes;

/// <summary>
/// An attribute to denote the generation of a visitor.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
internal class VisitorAttribute : Attribute
{
    /// <summary>
    /// The type of the node the visitor visits.
    /// </summary>
    public Type NodeType { get; set; }

    /// <summary>
    /// The type to return from the visitor calls.
    /// </summary>
    public Type? ReturnType { get; set; }

    /// <summary>
    /// The generic type name to return from the visitor calls.
    /// </summary>
    public string? GenericReturnType { get; set; }

    /// <summary>
    /// The method name to use. The default is 'Visit'.
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisitorAttribute"/> class.
    /// </summary>
    /// <param name="nodeType">The type of the node the visitor visits.</param>
    /// <param name="returnType">The type to return from the visitor calls.</param>
    /// <param name="methodName">The method name to use.</param>
    public VisitorAttribute(Type nodeType, Type? returnType, string? methodName = null)
    {
        this.NodeType = nodeType;
        this.ReturnType = returnType;
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisitorAttribute"/> class.
    /// </summary>
    /// <param name="nodeType">The type of the node the visitor visits.</param>
    public VisitorAttribute(Type nodeType)
        : this(nodeType, null)
    {
    }
}
