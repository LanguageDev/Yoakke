using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class BasicTests
    {
        [Lexer("BasicLexer")]
        public enum TokenType
        {
            [Ignore] [Regex("[ \t\r\n]")] Whitespace,

            [Error] Error,
            [End] EndOfFile,

            [Token("if")] KwIf,
            [Regex(Regex.Ident)] Ident,
            [Token("+")] Plus,
            [Token("-")] Minus,
            [Token("*")] Star,
            [Token("/")] Slash,
            [Token("(")] OpenParen,
            [Token(")")] CloseParen,
        }

        [TestMethod]
        public void TestMethod1()
        {
            var lexer = new BasicLexer("a + b");

            while (true)
            {
                var t = lexer.Next();
                if (t.Kind == TokenType.EndOfFile) break;
            }
        }
    }
}
