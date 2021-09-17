// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class RingBufferTests
    {
        [TestMethod]
        public void EmptyBuffer()
        {
            var rb = new RingBuffer<int>();

            Assert.AreEqual(0, rb.Count);
            Assert.AreEqual(0, rb.Head);
            Assert.AreEqual(0, rb.Tail);
        }

        [TestMethod]
        public void IndexEmptyBuffer()
        {
            var rb = new RingBuffer<int>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => rb[0]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => rb[0] = 0);
        }

        [TestMethod]
        public void AddingElementToBack()
        {
            var rb = new RingBuffer<int>();

            rb.AddBack(1);

            Assert.AreEqual(1, rb.Count);
            Assert.AreEqual(0, rb.Head);
            Assert.AreEqual(1, rb.Tail);
            Assert.AreEqual(1, rb[0]);
        }

        [TestMethod]
        public void AddingElementToFront()
        {
            var rb = new RingBuffer<int>();

            rb.AddFront(1);

            Assert.AreEqual(1, rb.Count);
            Assert.AreEqual(rb.Capacity - 1, rb.Head);
            Assert.AreEqual(0, rb.Tail);
            Assert.AreEqual(1, rb[0]);
        }

        [TestMethod]
        public void AddingElementToFrontAndBack()
        {
            var rb = new RingBuffer<int>();

            rb.AddFront(2);
            rb.AddBack(1);

            Assert.AreEqual(2, rb.Count);
            Assert.AreEqual(rb.Capacity - 1, rb.Head);
            Assert.AreEqual(1, rb.Tail);
            Assert.AreEqual(2, rb[0]);
            Assert.AreEqual(1, rb[1]);
        }

        [TestMethod]
        public void InsteringElementIntoEmpty()
        {
            var rb = new RingBuffer<int>();

            rb.Insert(0, 1);

            Assert.AreEqual(1, rb.Count);
            Assert.AreEqual(1, rb[0]);
        }

        [TestMethod]
        public void RemovingElementFromBack()
        {
            var rb = new RingBuffer<int>();

            rb.AddBack(1);
            rb.AddBack(2);
            var removed = rb.RemoveBack();

            Assert.AreEqual(1, rb.Count);
            Assert.AreEqual(0, rb.Head);
            Assert.AreEqual(1, rb.Tail);
            Assert.AreEqual(1, rb[0]);
            Assert.AreEqual(2, removed);
        }

        [TestMethod]
        public void RemovingElementFromFront()
        {
            var rb = new RingBuffer<int>();

            rb.AddBack(1);
            rb.AddBack(2);
            var removed = rb.RemoveFront();

            Assert.AreEqual(1, rb.Count);
            Assert.AreEqual(1, rb.Head);
            Assert.AreEqual(2, rb.Tail);
            Assert.AreEqual(2, rb[0]);
            Assert.AreEqual(1, removed);
        }

        [TestMethod]
        public void RemovingElementFromBackEmpty()
        {
            var rb = new RingBuffer<int>();

            Assert.ThrowsException<InvalidOperationException>(() => rb.RemoveBack());
        }

        [TestMethod]
        public void RemovingElementFromFrontEmpty()
        {
            var rb = new RingBuffer<int>();

            Assert.ThrowsException<InvalidOperationException>(() => rb.RemoveFront());
        }

        [TestMethod]
        public void ChangingContentsWithIndexer()
        {
            var rb = new RingBuffer<int>();

            rb.AddBack(1);
            rb.AddBack(2);
            rb.AddBack(3);
            rb[2] = 4;

            Assert.AreEqual(3, rb.Count);
            Assert.AreEqual(0, rb.Head);
            Assert.AreEqual(3, rb.Tail);
            Assert.AreEqual(1, rb[0]);
            Assert.AreEqual(2, rb[1]);
            Assert.AreEqual(4, rb[2]);
        }
    }
}
