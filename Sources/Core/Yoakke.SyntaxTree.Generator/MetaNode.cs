// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator;

/// <summary>
/// A helper to build a hierarchy of syntax tree nodes.
/// </summary>
internal class MetaNode
{
  /// <summary>
  /// The parent of this node.
  /// </summary>
  public MetaNode? Parent { get; set; }

  /// <summary>
  /// The node symbol.
  /// </summary>
  public INamedTypeSymbol NodeClass { get; }

  /// <summary>
  /// The child nodes.
  /// </summary>
  public IList<MetaNode> Children { get; } = new List<MetaNode>();

  /// <summary>
  /// The different overrides for different visitors.
  /// </summary>
  public IDictionary<Visitor, VisitorOverride> VisitorOverrides { get; } = new Dictionary<Visitor, VisitorOverride>();

  /// <summary>
  /// Initializes a new instance of the <see cref="MetaNode"/> class.
  /// </summary>
  /// <param name="nodeClass">The node class symbol.</param>
  public MetaNode(INamedTypeSymbol nodeClass)
  {
    this.NodeClass = nodeClass;
  }

  /// <summary>
  /// Adds a child node to this one.
  /// </summary>
  /// <param name="child">The child node to add.</param>
  public void AddChild(MetaNode child)
  {
    if (child.Parent is not null && child.Parent != this)
    {
      throw new InvalidOperationException("Re-setting parent to something invalid.");
    }

    this.Children.Add(child);
    child.Parent = this;
  }
}
