// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Utilities.Intervals;

namespace Yoakke.Utilities.Tests
{
    [TestClass]
    public class IntervalTests : IntervalTestBase
    {
        [TestMethod]
        public void FirstBeforeSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Disjunct(Iv("1..4"), Iv("5..7")),
                Rel("1..4", "5..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Disjunct(Iv("1..4"), Iv("5..7")),
                Rel("5..7", "1..4"));
        }

        [TestMethod]
        public void FirstTouchesSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Touching(Iv("1..4"), Iv("4..7")),
                Rel("1..4", "4..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Touching(Iv("1..4"), Iv("4..7")),
                Rel("4..7", "1..4"));
        }

        [TestMethod]
        public void FirstStartsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Starting(Iv("4..6"), Iv("6..8")),
                Rel("4..8", "4..6"));
            Assert.AreEqual(
                new IntervalRelation<int>.Starting(Iv("4..6"), Iv("6..8")),
                Rel("4..6", "4..8"));
        }

        [TestMethod]
        public void FirstFinishesSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Finishing(Iv("4..6"), Iv("6..8")),
                Rel("6..8", "4..8"));
            Assert.AreEqual(
                new IntervalRelation<int>.Finishing(Iv("4..6"), Iv("6..8")),
                Rel("4..8", "6..8"));
        }

        [TestMethod]
        public void SingletonIntersectionRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping(
                    Iv("4..6"),
                    Iv("6..=6"),
                    new Interval<int>(LowerBound<int>.Exclusive(6), UpperBound<int>.Exclusive(8))),
                Rel("4..=6", "6..8"));
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping(
                    Iv("4..6"),
                    Iv("6..=6"),
                    new Interval<int>(LowerBound<int>.Exclusive(6), UpperBound<int>.Exclusive(8))),
                Rel("6..8", "4..=6"));
        }

        [TestMethod]
        public void FirstContainsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Containing(Iv("2..4"), Iv("4..7"), Iv("7..10")),
                Rel("2..10", "4..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Containing(Iv("2..4"), Iv("4..7"), Iv("7..10")),
                Rel("4..7", "2..10"));
        }

        [TestMethod]
        public void FirstIntersectsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping(Iv("2..4"), Iv("4..7"), Iv("7..9")),
                Rel("2..7", "4..9"));
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping(Iv("2..4"), Iv("4..7"), Iv("7..9")),
                Rel("4..9", "2..7"));
        }
    }
}
