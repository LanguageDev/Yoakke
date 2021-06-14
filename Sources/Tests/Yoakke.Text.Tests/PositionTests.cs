// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Text.Tests
{
    [TestClass]
    public class PositionTests
    {
        public static IReadOnlyList<object[]> EqualPositions { get; } = new object[][]
        {
            new object[] { new Position(0, 0), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 3) },
            new object[] { new Position(3, 3), new Position(3, 3) },
        };

        public static IReadOnlyList<object[]> UnequalPositions { get; } = new object[][]
        {
            new object[] { new Position(0, 0), new Position(0, 1) },
            new object[] { new Position(0, 0), new Position(1, 0) },
            new object[] { new Position(0, 3), new Position(1, 3) },
            new object[] { new Position(3, 3), new Position(4, 3) },
            new object[] { new Position(3, 3), new Position(4, 5) },
            new object[] { new Position(3, 3), new Position(2, 1) },
        };

        [DataTestMethod]
        [DynamicData(nameof(EqualPositions))]
        public void Equals(Position p1, Position p2)
        {
            Assert.AreEqual(p1, p2);
            Assert.AreEqual(p1, (object)p2);
            Assert.IsTrue(p1 == p2);
            Assert.IsFalse(p1 != p2);
            Assert.IsTrue(p1 >= p2);
            Assert.IsTrue(p1 <= p2);
            Assert.IsFalse(p1 > p2);
            Assert.IsFalse(p1 < p2);
            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
        }

        [DataTestMethod]
        [DynamicData(nameof(UnequalPositions))]
        public void NotEquals(Position p1, Position p2)
        {
            Assert.AreNotEqual(p1, p2);
            Assert.AreNotEqual(p1, (object)p2);
            Assert.IsFalse(p1 == p2);
            Assert.IsTrue(p1 != p2);
            Assert.IsTrue(p1 > p2 || p1 < p2);
            Assert.IsTrue(p1 >= p2 || p1 <= p2);
            Assert.IsTrue((p1 >= p2) != (p1 <= p2));
        }
    }
}
