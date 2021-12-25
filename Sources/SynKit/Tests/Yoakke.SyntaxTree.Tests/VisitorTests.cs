using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.SyntaxTree.Tests;

public partial class VisitorTests
{
  [SyntaxTree]
  internal abstract partial record Ast
  {
    public partial record Node1(int Foo, int Bar) : Ast;

    public partial record Node2(IList<Ast> Values) : Ast;

    [SyntaxTreeIgnore]
    public partial record Node3(int Baz) : Ast
    {
      public override int ChildCount => throw new NotImplementedException();

      public override object GetChild(int index) => throw new NotImplementedException();
    }
  }

  [Visitor(typeof(Ast))]
  internal abstract partial class VoidVisitorBase
  {
  }

  internal class MyVoidVisitor : VoidVisitorBase
  {
    public StringBuilder Text { get; set; } = new();

    public void Call(Ast n) => this.Visit(n);

    protected override void Visit(Ast.Node1 n)
    {
      this.Text.Append($"N1[{n.Foo}, {n.Bar}](");
      base.Visit(n);
      this.Text.Append(')');
    }

    protected override void Visit(Ast.Node2 n)
    {
      this.Text.Append($"N2(");
      base.Visit(n);
      this.Text.Append(')');
    }
  }

  [Visitor(typeof(Ast), typeof(string))]
  internal abstract partial class StrVisitorBase
  {
  }

  internal class MyStrVisitor : StrVisitorBase
  {
    public string Call(Ast n) => this.Visit(n);

    protected override string Visit(Ast.Node1 n) => $"N1({n.Foo}, {n.Bar})";

    protected override string Visit(Ast.Node2 n) => $"N2({string.Join(", ", n.Values.Select(this.Visit))})";
  }

  [Fact]
  public void VoidVisitorBasic()
  {
    var ast = new Ast.Node2(new Ast[]
    {
                new Ast.Node1(1, 2),
                new Ast.Node2(new Ast[] { new Ast.Node1(3, 4) }),
                new Ast.Node1(5, 6),
    });

    var visitor = new MyVoidVisitor();
    visitor.Call(ast);

    Assert.Equal("N2(N1[1, 2]()N2(N1[3, 4]())N1[5, 6]())", visitor.Text.ToString());
  }

  [Fact]
  public void VoidVisitorNotSupported()
  {
    var ast = new Ast.Node3(1);

    var visitor = new MyVoidVisitor();

    Assert.Throws<NotSupportedException>(() => visitor.Call(ast));
  }

  [Fact]
  public void StrVisitorBasic()
  {
    var ast = new Ast.Node2(new Ast[]
    {
                new Ast.Node1(1, 2),
                new Ast.Node2(new Ast[] { new Ast.Node1(3, 4) }),
                new Ast.Node1(5, 6),
    });

    var visitor = new MyStrVisitor();
    var result = visitor.Call(ast);

    Assert.Equal("N2(N1(1, 2), N2(N1(3, 4)), N1(5, 6))", result);
  }

  [Fact]
  public void StrVisitorNotSupported()
  {
    var ast = new Ast.Node3(1);

    var visitor = new MyStrVisitor();

    Assert.Throws<NotSupportedException>(() => visitor.Call(ast));
  }
}
