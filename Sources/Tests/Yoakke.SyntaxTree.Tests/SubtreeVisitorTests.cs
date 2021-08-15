// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.SyntaxTree.Tests
{
    [TestClass]
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

        [TestMethod]
        public void FullHierarchyVisitorTests()
        {
            var type = typeof(FullHierarchyVisitor);

            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
        }

        [TestMethod]
        public void StatementSubtreeVisitorTests()
        {
            var type = typeof(StatementSubtreeVisitor);

            Assert.IsNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
            Assert.IsNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
        }

        [TestMethod]
        public void StatementAndExpressionSubtreeVisitorTests()
        {
            var type = typeof(StatementAndExpressionSubtreeVisitor);

            Assert.IsNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast) }, null));
            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Statement) }, null));
            Assert.IsNotNull(type.GetMethod("Visit", Flags, null, new[] { typeof(Ast.Expression) }, null));
        }
    }
}
