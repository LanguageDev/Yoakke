// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Lexer.Tests
{
    // https://github.com/LanguageDev/Yoakke/issues/17
    [TestClass]
    public partial class Issue17Tests
    {
        internal enum TokenType
        {
            [Error] Error,
            [End] End,
        }

        [Lexer(typeof(TokenType))]
        internal partial class ImplicitCtorLexer
        {
        }

        [Lexer(typeof(TokenType))]
        internal partial class ExplicitCtorLexer
        {
            public ExplicitCtorLexer(string text)
                : base(text)
            {
            }
        }

        [TestMethod]
        public void ImplicitCtors()
        {
            Assert.AreEqual(2, typeof(ImplicitCtorLexer).GetConstructors().Length);
            Assert.IsNotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(string) }));
            Assert.IsNotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(TextReader) }));
        }

        [TestMethod]
        public void ExplicitCtors()
        {
            Assert.AreEqual(1, typeof(ExplicitCtorLexer).GetConstructors().Length);
            Assert.IsNotNull(typeof(ExplicitCtorLexer).GetConstructor(new[] { typeof(string) }));
            Assert.IsNull(typeof(ExplicitCtorLexer).GetConstructor(new[] { typeof(TextReader) }));
        }
    }
}
