using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class PeekableEnumeratorTests
    {
        [TestMethod]
        public void CurrentOnEmpty()
        {
            var l = new List<int> { };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.ThrowsException<InvalidOperationException>(() => e.Current);
        }

        [TestMethod]
        public void TryPeekThenCurrentOnEmpty()
        {
            var l = new List<int> { };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            e.TryPeek(4, out var _);

            Assert.ThrowsException<InvalidOperationException>(() => e.Current);
        }

        [TestMethod]
        public void MoveNextOnEmpty()
        {
            var l = new List<int> { };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.IsFalse(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        public void CurrentBeforeMoveNext()
        {
            var l = new List<int> { 1, 2, 3 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.ThrowsException<InvalidOperationException>(() => e.Current);
        }

        [TestMethod]
        public void TryPeekCurrentBeforeMoveNext()
        {
            var l = new List<int> { 1, 2, 3 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            e.TryPeek(4, out var _);

            Assert.ThrowsException<InvalidOperationException>(() => e.Current);
        }

        [TestMethod]
        public void CurrentAfterLast()
        {
            var l = new List<int> { 1 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            e.MoveNext();
            e.MoveNext();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => e.Current);
        }

        [TestMethod]
        public void SingleElement()
        {
            var l = new List<int> { 1 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Peek(0));
            Assert.AreEqual(1, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        public void TryPeekThenSingleElement()
        {
            var l = new List<int> { 1 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            e.TryPeek(4, out var _);

            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Peek(0));
            Assert.AreEqual(1, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [TestMethod]
        public void PeekAfterLast()
        {
            var l = new List<int> { 1 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => e.Peek(3));
        }

        [TestMethod]
        public void Peek1BeforeFirstMoveNext()
        {
            var l = new List<int> { 1 };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.AreEqual(e.Peek(1), 1);
        }

        [TestMethod]
        public void Peek1BeforeFirstMoveNextEmpty()
        {
            var l = new List<int> { };
            IPeekableEnumerator<int> e = new PeekableEnumerator<int>(l.GetEnumerator());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => e.Peek(1));
        }
    }
}
