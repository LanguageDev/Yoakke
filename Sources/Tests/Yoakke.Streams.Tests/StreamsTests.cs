using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Streams.Tests
{
    [TestClass]
    public class StreamsTests
    {
        [TestMethod]
        public void MemoryStreamSequence()
        {
            var ms = new MemoryStream<int>(new int[] { 1, 2, 3 }.AsMemory());

            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(1, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(2, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(3, ms.Consume());
            Assert.IsTrue(ms.IsEnd);
        }

        [TestMethod]
        public void FilterStream()
        {
            var ms = new MemoryStream<int>(new int[] { 1, 2, 3, 4, 5, 6 }.AsMemory()).Filter(n => n % 2 == 1);

            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(1, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(3, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(5, ms.Consume());
            Assert.IsTrue(ms.IsEnd);
        }

        [TestMethod]
        public void BufferedStream()
        {
            var ms = new MemoryStream<int>(new int[] { 1, 2, 3, 4, 5, 6 }.AsMemory()).Filter(n => n % 2 == 1).ToBuffered();

            Assert.IsFalse(ms.IsEnd);
            Assert.IsTrue(ms.TryPeek(out var t0));
            Assert.AreEqual(t0, 1);
            Assert.IsTrue(ms.TryLookAhead(1, out var t1));
            Assert.AreEqual(t1, 3);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(1, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(3, ms.Consume());
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(5, ms.Consume());
            Assert.IsTrue(ms.IsEnd);
        }
    }
}
