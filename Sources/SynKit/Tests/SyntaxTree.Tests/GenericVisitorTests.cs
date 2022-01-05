// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SynKit.SyntaxTree.Attributes;

namespace Yoakke.SynKit.SyntaxTree.Tests;

public partial class GenericVisitorTests
{
    [SyntaxTree]
    internal abstract partial record Ast
    {
        public partial record Foo : Ast;

        public partial record Bar : Ast;
    }

    [Visitor(typeof(Ast), GenericReturnType = nameof(TReturn))]
    internal abstract partial class GenericVisitorBase<TReturn>
    {
    }

    private class IntVisitor : GenericVisitorBase<int>
    {
        public int GetInt(Ast ast) => this.Visit(ast);

        protected override int Visit(Ast.Foo foo) => 1;

        protected override int Visit(Ast.Bar bar) => 2;
    }

    private class StringVisitor : GenericVisitorBase<string>
    {
        public string GetString(Ast ast) => this.Visit(ast);

        protected override string Visit(Ast.Foo foo) => "foo";

        protected override string Visit(Ast.Bar bar) => "bar";
    }

    [Fact]
    public void IntVisitorTests()
    {
        var visitor = new IntVisitor();
        Assert.Equal(1, visitor.GetInt(new Ast.Foo()));
        Assert.Equal(2, visitor.GetInt(new Ast.Bar()));
    }

    [Fact]
    public void StringVisitorTests()
    {
        var visitor = new StringVisitor();
        Assert.Equal("foo", visitor.GetString(new Ast.Foo()));
        Assert.Equal("bar", visitor.GetString(new Ast.Bar()));
    }
}
