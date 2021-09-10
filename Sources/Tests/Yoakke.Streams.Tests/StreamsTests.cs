using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.AreEqual(ms.Consume(), 1);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 2);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 3);
            Assert.IsTrue(ms.IsEnd);
        }

        [TestMethod]
        public void FilterStream()
        {
            var ms = new MemoryStream<int>(new int[] { 1, 2, 3, 4, 5, 6 }.AsMemory()).Filter(n => n % 2 == 1);

            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 1);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 3);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 5);
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
            Assert.AreEqual(ms.Consume(), 1);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 3);
            Assert.IsFalse(ms.IsEnd);
            Assert.AreEqual(ms.Consume(), 5);
            Assert.IsTrue(ms.IsEnd);
        }
    }
}
