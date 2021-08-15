// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.SyntaxTree.Tests
{
    [TestClass]
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

        [TestMethod]
        public void IntVisitorTests()
        {
            var visitor = new IntVisitor();
            Assert.AreEqual(1, visitor.GetInt(new Ast.Foo()));
            Assert.AreEqual(2, visitor.GetInt(new Ast.Bar()));
        }

        [TestMethod]
        public void StringVisitorTests()
        {
            var visitor = new StringVisitor();
            Assert.AreEqual("foo", visitor.GetString(new Ast.Foo()));
            Assert.AreEqual("bar", visitor.GetString(new Ast.Bar()));
        }
    }
}
