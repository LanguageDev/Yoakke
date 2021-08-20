// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer;
using Yoakke.Parser.Attributes;

namespace Yoakke.Parser.Tests
{
    // https://github.com/LanguageDev/Yoakke/issues/17
    [TestClass]
    public partial class Issue17Tests
    {
        [Parser]
        internal partial class ImplicitCtorParser
        {
        }

        [Parser]
        internal partial class ExplicitCtorParser
        {
            public ExplicitCtorParser(IEnumerable<IToken> tokens)
                : base(tokens)
            {
            }
        }

        [TestMethod]
        public void ImplicitCtors()
        {
            Assert.AreEqual(2, typeof(ImplicitCtorParser).GetConstructors().Length);
            Assert.IsNotNull(typeof(ImplicitCtorParser).GetConstructor(new[] { typeof(IEnumerable<IToken>) }));
            Assert.IsNotNull(typeof(ImplicitCtorParser).GetConstructor(new[] { typeof(ILexer) }));
        }

        [TestMethod]
        public void ExplicitCtors()
        {
            Assert.AreEqual(1, typeof(ExplicitCtorParser).GetConstructors().Length);
            Assert.IsNotNull(typeof(ExplicitCtorParser).GetConstructor(new[] { typeof(IEnumerable<IToken>) }));
            Assert.IsNull(typeof(ExplicitCtorParser).GetConstructor(new[] { typeof(ILexer) }));
        }
    }
}