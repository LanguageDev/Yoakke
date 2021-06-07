using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class TokenTests : TestBase<int>
    {
        [TestMethod]
        public void Equality()
        {
            var t1 = Token("hello", 3, Range((3, 4), 5));
            var t2 = Token("hello", 3, Range((3, 4), 5));
            Assert.AreEqual(t1, t2);
        }

        [TestMethod]
        public void InequalityContent()
        {
            var t1 = Token("hello", 3, Range((3, 4), 5));
            var t2 = Token("bye", 3, Range((3, 4), 5));
            Assert.AreNotEqual(t1, t2);
        }

        [TestMethod]
        public void InequalityKind()
        {
            var t1 = Token("hello", 3, Range((3, 4), 5));
            var t2 = Token("hello", 4, Range((3, 4), 5));
            Assert.AreNotEqual(t1, t2);
        }

        [TestMethod]
        public void InequalityPosition()
        {
            var t1 = Token("hello", 3, Range((3, 4), 5));
            var t2 = Token("hello", 4, Range((4, 4), 5));
            Assert.AreNotEqual(t1, t2);
        }

        [TestMethod]
        public void InequalityLength()
        {
            var t1 = Token("hello", 3, Range((3, 4), 5));
            var t2 = Token("hello", 4, Range((3, 4), 6));
            Assert.AreNotEqual(t1, t2);
        }
    }
}
