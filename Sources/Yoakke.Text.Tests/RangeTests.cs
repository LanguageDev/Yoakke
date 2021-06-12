// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Text.Tests
{
    [TestClass]
    public class RangeTests
    {
        public static IReadOnlyList<object[]> InvalidPositionPairs { get; } = new object[][]
        {
            new object[] { new Position(0, 1), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 2) },
            new object[] { new Position(4, 3), new Position(3, 3) },
            new object[] { new Position(4, 3), new Position(3, 5) },
            new object[] { new Position(4, 3), new Position(4, 1) },
        };

        public static IReadOnlyList<object[]> EqualRanges { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 3)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 2), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 2), 0), new Range(new Position(0, 2), new Position(0, 2)) },
            new object[] { new Range(new Position(1, 3), 5), new Range(new Position(1, 3), new Position(1, 8)) },
        };

        public static IReadOnlyList<object[]> UnequalRanges { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 4)) },
            new object[] { new Range(new Position(0, 1), 3), new Range(new Position(0, 2), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 2), 0), new Range(new Position(1, 2), new Position(2, 2)) },
            new object[] { new Range(new Position(1, 4), 4), new Range(new Position(1, 3), new Position(1, 8)) },
        };

        public static IReadOnlyList<object[]> ContainedPoints { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 0) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 1) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 2) },

            new object[] { new Range(new Position(1, 3), 5), new Position(1, 3) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 5) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 7) },
        };

        public static IReadOnlyList<object[]> NotContainedPoints { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 3) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 4) },
            new object[] { new Range(new Position(0, 0), 3), new Position(1, 2) },
            new object[] { new Range(new Position(0, 0), 3), new Position(1, 0) },

            new object[] { new Range(new Position(1, 3), 5), new Position(0, 3) },
            new object[] { new Range(new Position(1, 3), 5), new Position(0, 5) },
            new object[] { new Range(new Position(1, 3), 5), new Position(0, 7) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 2) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 8) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 0) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 10) },
        };

        public static IReadOnlyList<object[]> IntersectingRanges { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 3)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 1), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 1), new Position(0, 2)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 0), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 4), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 4), new Position(0, 8)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 2), new Position(0, 8)) },
        };

        public static IReadOnlyList<object[]> NotIntersectingRanges { get; } = new object[][]
        {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 0)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 1), new Position(0, 2)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 5), new Position(0, 7)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(1, 0), new Position(1, 3)) },
        };

        [DataTestMethod]
        [DynamicData(nameof(InvalidPositionPairs))]
        public void InvalidConstruction(Position start, Position end) => 
            Assert.ThrowsException<ArgumentException>(() => new Range(start, end));

        [DataTestMethod]
        [DynamicData(nameof(EqualRanges))]
        public void Equals(Range r1, Range r2)
        {
            Assert.AreEqual(r1, r2);
            Assert.AreEqual(r1, (object)r2);
            Assert.IsTrue(r1 == r2);
            Assert.IsFalse(r1 != r2);
            Assert.AreEqual(r1.GetHashCode(), r2.GetHashCode());
        }

        [DataTestMethod]
        [DynamicData(nameof(UnequalRanges))]
        public void NotEquals(Range r1, Range r2)
        {
            Assert.AreNotEqual(r1, r2);
            Assert.AreNotEqual(r1, (object)r2);
            Assert.IsTrue(r1 != r2);
            Assert.IsFalse(r1 == r2);
        }

        [DataTestMethod]
        [DynamicData(nameof(ContainedPoints))]
        public void Contains(Range r, Position p) => Assert.IsTrue(r.Contains(p));

        [DataTestMethod]
        [DynamicData(nameof(NotContainedPoints))]
        public void NotContains(Range r, Position p) => Assert.IsFalse(r.Contains(p));

        [DataTestMethod]
        [DynamicData(nameof(IntersectingRanges))]
        public void Intersects(Range r1, Range r2)
        {
            Assert.IsTrue(r1.Intersects(r2));
            Assert.IsTrue(r2.Intersects(r1));
        }

        [DataTestMethod]
        [DynamicData(nameof(NotIntersectingRanges))]
        public void NotIntersects(Range r1, Range r2)
        {
            Assert.IsFalse(r1.Intersects(r2));
            Assert.IsFalse(r2.Intersects(r1));
        }
    }
}
