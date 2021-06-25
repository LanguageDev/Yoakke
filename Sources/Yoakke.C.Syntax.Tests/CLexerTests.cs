using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Text;
using Kind = Yoakke.C.Syntax.CTokenType;

namespace Yoakke.C.Syntax.Tests
{
    [TestClass]
    public class CLexerTests
    {
        private static IEnumerable<object[]> AllTokensInput { get; } = new object[][]
        {
            new object[] { Tok(Kind.KeywordAuto, "auto"), "auto" },
            new object[] { Tok(Kind.KeywordBreak, "break"), "break" },
            new object[] { Tok(Kind.KeywordCase, "case"), "case" },
            new object[] { Tok(Kind.KeywordChar, "char"), "char" },
            // new object[] { Tok(Kind.KeywordChar, Rn(0, 0, 4), "char"), "char" },
        };

        private static Range Rn(int line, int column, int length) => new(new(line, column), length);

        private static CToken Tok(Kind ty, string t) => Tok(ty, Rn(0, 0, t.Length), t);

        private static CToken Tok(Kind ty, Range r, string t) => Tok(ty, r, t, r, t);

        private static CToken Tok(Kind ty, Range r, string t, Range logicalR, string logicalT) => new(r, t, logicalR, logicalT, ty);

        [DynamicData(nameof(AllTokensInput))]
        [DataTestMethod]
        public void LexSingleToken(CToken expected, string text)
        {
            var lexer = new CLexer(text);
            var token = lexer.Next();
            var end = Tok(Kind.End, Rn(0, token.Text.Length, 0), string.Empty);
            Assert.AreEqual(expected, token);
            Assert.AreEqual(end, lexer.Next());
        }
    }
}
