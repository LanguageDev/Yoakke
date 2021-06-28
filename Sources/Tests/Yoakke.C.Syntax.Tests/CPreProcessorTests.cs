// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.C.Syntax.Tests
{
    [TestClass]
    public class CPreProcessorTests
    {
        private static IEnumerable<object[]> TextEqualsInputs { get; } = new object[][]
        {
            TextInput(
                "a b c",
                "a b c"),

            TextInput(
@"#define FOO
FOO",
string.Empty),

            TextInput(
@"#define FOO BAR
FOO",
"BAR"),

            TextInput(
@"#define FOO a b c
FOO",
"a b c"),

            TextInput(
@"#define FOO BAR
#define BAR QUX
FOO",
"QUX"),

            TextInput(
@"#define FOO(x) a x b
FOO(abc)",
"a abc b"),

            TextInput(
@"#define FOO(x) a x b
FOO((x, y, z))",
"a (x, y, z) b"),

            TextInput(
@"#define CAT(a, b) a ## b
CAT(ab, 2)
CAT(L, ""asd"")",
"ab2 L\"asd\""),

            TextInput(
@"#define FOO() A
#define LP() (
#define RP() )
FOO LP() RP()",
"FOO ( )"),

            TextInput(
@"#define FOO() A
#define EXPAND(x) x
#define LP() (
#define RP() )
EXPAND(FOO LP() RP())",
"A"),
        };

        private static object[] TextInput(string beforePP, string afterPP) => new object[] { beforePP, afterPP };

        [DynamicData(nameof(TextEqualsInputs))]
        [DataTestMethod]
        public void TextEquals(string beforePP, string afterPP)
        {
            var expectLexer = new CLexer(afterPP);
            var pp = new CPreProcessor(new CLexer(beforePP));
            while (true)
            {
                var expected = expectLexer.Next();
                var got = pp.Next();

                Assert.AreEqual(expected.Kind, got.Kind);
                Assert.AreEqual(expected.LogicalText, got.LogicalText);
                if (expected.Kind == CTokenType.End) break;
            }
        }
    }
}
