using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Parser.Tests
{
    [Parser]
    partial class MyParser
    {
        [Rule("statement : expression '+' expression")]
        public static void Foo() { }

        [Rule("expression : expression '+' expression")]
        public static void Addition() { }
    }

    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p = new MyParser();
            p.ParseStatement();
            p.ParseExpression();
        }
    }
}
