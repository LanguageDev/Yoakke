// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class IntervalMapTests : IntervalTestBase
    {
        private class TestEqualityComparer : IEqualityComparer<KeyValuePair<Interval<int>, List<int>>>
        {
            public static readonly TestEqualityComparer Default = new();

            public bool Equals(KeyValuePair<Interval<int>, List<int>> x, KeyValuePair<Interval<int>, List<int>> y) =>
                x.Key.Equals(y.Key) && x.Value.SequenceEqual(y.Value);

            public int GetHashCode([DisallowNull] KeyValuePair<Interval<int>, List<int>> obj)
            {
                throw new NotImplementedException();
            }
        }

        private static List<int> UnifyLists(List<int> l1, List<int> l2) => l1.Concat(l2).ToList();

        private static KeyValuePair<Interval<int>, List<int>> Kv(string iv, params int[] ints) =>
            new(Iv(iv), ints.ToList());

        private static List<KeyValuePair<Interval<int>, List<int>>> KvList(params KeyValuePair<Interval<int>, List<int>>[] kvs) =>
            kvs.ToList();

        private static List<int> VList(params int[] ints) => ints.ToList();

        [TestMethod]
        public void InsertIntoEmptyMap()
        {
            var map = new IntervalMap<int, List<int>>();

            map.AddAndUpdate(Iv("2..3"), new List<int> { 1 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(Kv("2..3", 1)), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctBefore()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("2..3"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("2..3", 2),
                Kv("5..7", 1),
                Kv("12..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctBeforeTouch()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("2..5"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("2..5", 2),
                Kv("5..7", 1),
                Kv("12..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctBetween()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("9..11"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1),
                Kv("9..11", 2),
                Kv("12..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctBetweenTouch()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("7..12"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1),
                Kv("7..12", 2),
                Kv("12..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctAfter()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("17..19"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1),
                Kv("12..15", 1),
                Kv("17..19", 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapDisjunctAfterTouch()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)),
                ("12..15", VList(1)));

            map.AddAndUpdate(Iv("15..19"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1),
                Kv("12..15", 1),
                Kv("15..19", 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleEqual()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)));

            map.AddAndUpdate(Iv("5..7"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1, 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleContainingInserted()
        {
            var map = IvMap(UnifyLists,
                ("3..9", VList(1)));

            map.AddAndUpdate(Iv("5..7"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("3..5", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleContainingExisting()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)));

            map.AddAndUpdate(Iv("3..9"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("3..5", 2),
                Kv("5..7", 1, 2),
                Kv("7..9", 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleOverlappingLeft()
        {
            var map = IvMap(UnifyLists,
                ("5..9", VList(1)));

            map.AddAndUpdate(Iv("2..7"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("2..5", 2),
                Kv("5..7", 1, 2),
                Kv("7..9", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleOverlappingRight()
        {
            var map = IvMap(UnifyLists,
                ("5..9", VList(1)));

            map.AddAndUpdate(Iv("7..12"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1),
                Kv("7..9", 1, 2),
                Kv("9..12", 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleStartingContainingInserted()
        {
            var map = IvMap(UnifyLists,
                ("5..11", VList(1)));

            map.AddAndUpdate(Iv("5..7"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1, 2),
                Kv("7..11", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleStartingContainingExisting()
        {
            var map = IvMap(UnifyLists,
                ("5..7", VList(1)));

            map.AddAndUpdate(Iv("5..11"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..7", 1, 2),
                Kv("7..11", 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleFinishingContainingInserted()
        {
            var map = IvMap(UnifyLists,
                ("5..11", VList(1)));

            map.AddAndUpdate(Iv("8..11"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..8", 1),
                Kv("8..11", 1, 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapSingleFinishingContainingExisting()
        {
            var map = IvMap(UnifyLists,
                ("8..11", VList(1)));

            map.AddAndUpdate(Iv("5..11"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("5..8", 2),
                Kv("8..11", 1, 2)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapCoverTwoExactly()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)));

            map.AddAndUpdate(Iv("5..12"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..12", 1, 2),
                Kv("14..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapCoverManyEvenExactly()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)),
                ("15..17", VList(1)),
                ("18..19", VList(1)));

            map.AddAndUpdate(Iv("5..17"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..12", 1, 2),
                Kv("12..14", 2),
                Kv("14..15", 1, 2),
                Kv("15..17", 1, 2),
                Kv("18..19", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapCoverManyOddExactly()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)),
                ("15..17", VList(1)),
                ("18..19", VList(1)),
                ("21..24", VList(1)));

            map.AddAndUpdate(Iv("5..19"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..12", 1, 2),
                Kv("12..14", 2),
                Kv("14..15", 1, 2),
                Kv("15..17", 1, 2),
                Kv("17..18", 2),
                Kv("18..19", 1, 2),
                Kv("21..24", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapCoverManyTouchingExactly()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("7..9", VList(1)),
                ("9..12", VList(1)),
                ("12..14", VList(1)),
                ("14..15", VList(1)),
                ("15..17", VList(1)),
                ("18..19", VList(1)));

            map.AddAndUpdate(Iv("5..17"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 1, 2),
                Kv("9..12", 1, 2),
                Kv("12..14", 1, 2),
                Kv("14..15", 1, 2),
                Kv("15..17", 1, 2),
                Kv("18..19", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapTwoStartIntersect()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)));

            map.AddAndUpdate(Iv("5..10"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..10", 1, 2),
                Kv("10..12", 1),
                Kv("14..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapTwoIntersectFinish()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)));

            map.AddAndUpdate(Iv("6..12"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..6", 1),
                Kv("6..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..12", 1, 2),
                Kv("14..15", 1)
                ), TestEqualityComparer.Default));
        }

        [TestMethod]
        public void InsertIntoMapTwoIntersectIntersect()
        {
            var map = IvMap(UnifyLists,
                ("1..3", VList(1)),
                ("5..7", VList(1)),
                ("9..12", VList(1)),
                ("14..15", VList(1)));

            map.AddAndUpdate(Iv("6..10"), new List<int> { 2 }, UnifyLists);

            Assert.IsTrue(map.SequenceEqual(KvList(
                Kv("1..3", 1),
                Kv("5..6", 1),
                Kv("6..7", 1, 2),
                Kv("7..9", 2),
                Kv("9..10", 1, 2),
                Kv("10..12", 1),
                Kv("14..15", 1)
                ), TestEqualityComparer.Default));
        }
    }
}
