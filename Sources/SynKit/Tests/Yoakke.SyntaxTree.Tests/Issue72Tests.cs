// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SyntaxTree.Attributes;
using Yoakke.SyntaxTree.Tests.Tree;

namespace Yoakke.SyntaxTree.Tests;

// https://github.com/LanguageDev/Yoakke/issues/72
public partial class Issue72Tests
{
    [Visitor(typeof(ExternalAst), ReturnType = typeof(string))]
    internal abstract partial class ExternalVisitorBase
    {
    }

    internal partial class ExternalVisitor : ExternalVisitorBase
    {
        public string Stringify(ExternalAst ast) => this.Visit(ast);

        protected override string Visit(ExternalAst.Foo foo) => "got a foo";

        protected override string Visit(ExternalAst.Bar bar) => "got a bar";

        protected override string Visit(ExternalAst.Baz baz) => "got a baz";
    }

    [Fact]
    public void TestVisitingFoo()
    {
        var visitor = new ExternalVisitor();
        ExternalAst ast = new ExternalAst.Foo();
        Assert.Equal("got a foo", visitor.Stringify(ast));
    }

    [Fact]
    public void TestVisitingBar()
    {
        var visitor = new ExternalVisitor();
        ExternalAst ast = new ExternalAst.Bar();
        Assert.Equal("got a bar", visitor.Stringify(ast));
    }

    [Fact]
    public void TestVisitingBaz()
    {
        var visitor = new ExternalVisitor();
        ExternalAst ast = new ExternalAst.Baz();
        Assert.Equal("got a baz", visitor.Stringify(ast));
    }
}
