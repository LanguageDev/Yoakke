// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Reflection;
using Xunit;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.SyntaxTree.Tests;

public partial class SubtreeVisitorTests
{
  internal abstract partial record Ast
  {
    public partial record Statement : Ast;

    public partial record Expression : Ast;
  }

  [Visitor(typeof(Ast))]
  internal partial class FullHierarchyVisitor
  {
  }

  [Visitor(typeof(Ast.Statement))]
  internal partial class StatementSubtreeVisitor
  {
  }

  [Visitor(typeof(Ast.Statement))]
  [Visitor(typeof(Ast.Expression))]
  internal partial class StatementAndExpressionSubtreeVisitor
  {
  }

  private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

  [Fact]
  public void FullHierarchyVisitorTests()
  {
    var type = typeof(FullHierarchyVisitor);

    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
  }

  [Fact]
  public void StatementSubtreeVisitorTests()
  {
    var type = typeof(StatementSubtreeVisitor);

    Assert.Null(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
    Assert.Null(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
  }

  [Fact]
  public void StatementAndExpressionSubtreeVisitorTests()
  {
    var type = typeof(StatementAndExpressionSubtreeVisitor);

    Assert.Null(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
    Assert.NotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
  }
}
