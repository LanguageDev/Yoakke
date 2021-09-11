// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Streams;

namespace Yoakke.Parser.Tests
{
    [TestClass]
    public class CombinatorTests
    {
        [TestMethod]
        public void ItemParser()
        {
            var stream = new MemoryStream<char>("abc".AsMemory());
            var p = Combinator.Item<char>();

            var r1 = p.Parse(stream);
            var r2 = p.Parse(stream);
            var r3 = p.Parse(stream);
            var r4 = p.Parse(stream);

            Assert.IsTrue(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.IsTrue(r3.IsOk);
            Assert.IsFalse(r4.IsOk);
            Assert.AreEqual('a', r1.Ok.Value);
            Assert.AreEqual('b', r2.Ok.Value);
            Assert.AreEqual('c', r3.Ok.Value);
        }

        [TestMethod]
        public void ExactItemParser()
        {
            var stream = new MemoryStream<char>("abc".AsMemory());
            var p1 = Combinator.Item('a');
            var p2 = Combinator.Item('b');
            var p3 = Combinator.Item('d');
            var p4 = Combinator.Item('c');

            var r1 = p1.Parse(stream);
            var r2 = p2.Parse(stream);
            var r3 = p3.Parse(stream);
            var r4 = p4.Parse(stream);

            Assert.IsTrue(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.IsFalse(r3.IsOk);
            Assert.IsTrue(r4.IsOk);
            Assert.AreEqual('a', r1.Ok.Value);
            Assert.AreEqual('b', r2.Ok.Value);
            Assert.AreEqual('c', r4.Ok.Value);
        }

        [TestMethod]
        public void SequenceParser()
        {
            var stream = new MemoryStream<char>("abc".AsMemory());

            var p1 = Combinator.Seq(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('d'));
            var p2 = Combinator.Seq(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'));

            var r1 = p1.Parse(stream);
            var r2 = p2.Parse(stream);

            Assert.IsFalse(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.AreEqual(('a', 'b', 'c'), r2.Ok.Value);
        }

        [TestMethod]
        public void AlternativeParser()
        {
            var stream = new MemoryStream<char>("a".AsMemory());

            var p1 = Combinator.Alt(Combinator.Char('x'), Combinator.Char('y'), Combinator.Char('z'));
            var p2 = Combinator.Alt(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'));

            var r1 = p1.Parse(stream);
            var r2 = p2.Parse(stream);

            Assert.IsFalse(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.AreEqual('a', r2.Ok.Value);
        }

        [TestMethod]
        public void Repeat0Parser()
        {
            var emptyStream = new MemoryStream<char>(string.Empty.AsMemory());
            var nonEmptyStream = new MemoryStream<char>("aaa".AsMemory());

            var p = Combinator.Rep0(Combinator.Char('a'));

            var r1 = p.Parse(emptyStream);
            var r2 = p.Parse(nonEmptyStream);

            Assert.IsTrue(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.AreEqual(0, r1.Ok.Value.Count);
            Assert.AreEqual(3, r2.Ok.Value.Count);
            Assert.AreEqual('a', r2.Ok.Value[0]);
            Assert.AreEqual('a', r2.Ok.Value[1]);
            Assert.AreEqual('a', r2.Ok.Value[2]);
        }

        [TestMethod]
        public void Repeat1Parser()
        {
            var emptyStream = new MemoryStream<char>(string.Empty.AsMemory());
            var nonEmptyStream = new MemoryStream<char>("aaa".AsMemory());

            var p = Combinator.Rep1(Combinator.Char('a'));

            var r1 = p.Parse(emptyStream);
            var r2 = p.Parse(nonEmptyStream);

            Assert.IsFalse(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.AreEqual(3, r2.Ok.Value.Count);
            Assert.AreEqual('a', r2.Ok.Value[0]);
            Assert.AreEqual('a', r2.Ok.Value[1]);
            Assert.AreEqual('a', r2.Ok.Value[2]);
        }

        [TestMethod]
        public void OptParser()
        {
            var stream1 = new MemoryStream<char>("a".AsMemory());
            var stream2 = new MemoryStream<char>("b".AsMemory());

            var p = Combinator.Opt(Combinator.Char('a'));

            var r1 = p.Parse(stream1);
            var r2 = p.Parse(stream2);

            Assert.IsTrue(r1.IsOk);
            Assert.IsTrue(r2.IsOk);
            Assert.AreEqual('a', r1.Ok.Value);
            Assert.AreEqual(default, r2.Ok.Value);
        }

        [TestMethod]
        public void TransformParser()
        {
            var stream1 = new MemoryStream<char>("a".AsMemory());
            var stream2 = new MemoryStream<char>("b".AsMemory());

            var p = Combinator.Transform(Combinator.Char('a'), _ => 'b');

            var r1 = p.Parse(stream1);
            var r2 = p.Parse(stream2);

            Assert.IsTrue(r1.IsOk);
            Assert.IsFalse(r2.IsOk);
            Assert.AreEqual('b', r1.Ok.Value);
        }
    }
}
