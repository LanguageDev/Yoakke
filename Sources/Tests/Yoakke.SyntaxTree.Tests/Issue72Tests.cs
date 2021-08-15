// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.SyntaxTree.Attributes;
using Yoakke.SyntaxTree.Tests.Tree;

namespace Yoakke.SyntaxTree.Tests
{
    // https://github.com/LanguageDev/Yoakke/issues/72
    [TestClass]
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

        [TestMethod]
        public void TestVisitingFoo()
        {
            var visitor = new ExternalVisitor();
            ExternalAst ast = new ExternalAst.Foo();
            Assert.AreEqual("got a foo", visitor.Stringify(ast));
        }

        [TestMethod]
        public void TestVisitingBar()
        {
            var visitor = new ExternalVisitor();
            ExternalAst ast = new ExternalAst.Bar();
            Assert.AreEqual("got a bar", visitor.Stringify(ast));
        }

        [TestMethod]
        public void TestVisitingBaz()
        {
            var visitor = new ExternalVisitor();
            ExternalAst ast = new ExternalAst.Baz();
            Assert.AreEqual("got a baz", visitor.Stringify(ast));
        }
    }
}
