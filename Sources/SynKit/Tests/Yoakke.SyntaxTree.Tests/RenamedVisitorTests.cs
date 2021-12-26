// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.SyntaxTree.Tests;

public partial class RenamedVisitorTests
{
    [SyntaxTree]
    internal abstract partial record Ast
    {
        public partial record Node1(int Foo, int Bar) : Ast;

        public partial record Node2(IList<Ast> Values) : Ast;
    }

    [Visitor(typeof(Ast), ReturnType = typeof(string), MethodName = "Stringify")]
    internal partial class MyStrVisitor
    {
        public string Call(Ast n) => this.Stringify(n);

        protected string Stringify(Ast.Node1 n) => $"N1({n.Foo}, {n.Bar})";

        protected string Stringify(Ast.Node2 n) => $"N2({string.Join(", ", n.Values.Select(this.Stringify))})";
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
}
