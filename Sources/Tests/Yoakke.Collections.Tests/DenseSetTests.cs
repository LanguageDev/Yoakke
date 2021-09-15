// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class DenseSetTests
    {
        [DataTestMethod]
        [DataRow("", "4", "[4; 4]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "4", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "7", "(-oo; 5] U [7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "12", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "10", "(-oo; 5] U (7; 9) U [10; 10] U [12; 16]")]
        public void AddItem(string setText, string itemText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var item = int.Parse(itemText);

            originalSet.Add(item);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("", "4", "")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "4", "(-oo; 4) U (4; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "6", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "12", "(-oo; 5] U (7; 9) U (12; 16]")]
        public void RemoveItem(string setText, string itemText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var item = int.Parse(itemText);

            originalSet.Remove(item);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("", "(2; 4]", "(2; 4]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", "(-oo; 5] U [6; 6] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(3; 5]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(14; 15]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]", "(-oo; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(9; 13)", "(-oo; 5] U (7; 9) U (9; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(10; 11]", "(-oo; 5] U (7; 9) U (10; 11] U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(9; 11]", "(-oo; 5] U (7; 9) U (9; 11] U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(8; 12)", "(-oo; 5] U (7; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(4; 8)", "(-oo; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[5; 8]", "(-oo; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 13)", "(-oo; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", "(-oo; +oo)")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 24)", "(-oo; 24)")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-2; 24]", "(-oo; 24]")]
        public void AddInterval(string setText, string intervalText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var interval = Interval<int>.Parse(intervalText, int.Parse);

            originalSet.Add(interval);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("", "(2; 4]", "")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", "(-oo; 1) U (1; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(3; 5]", "(-oo; 3] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(14; 15]", "(-oo; 5] U (7; 9) U [12; 14] U (15; 16)")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)", "(-oo; 5] U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(9; 13)", "(-oo; 5] U (7; 9) U [13; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(10; 11]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(9; 11]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(8; 12)", "(-oo; 5] U (7; 8] U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(4; 8)", "(-oo; 4] U [8; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[5; 8]", "(-oo; 5) U (8; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 13)", "(-oo; 3) U [13; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", "")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 24)", "")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-2; 24]", "")]
        public void RemoveInterval(string setText, string intervalText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var interval = Interval<int>.Parse(intervalText, int.Parse);

            originalSet.Remove(interval);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)")]
        [DataRow("(-oo; +oo)", "")]
        [DataRow("(4; 23]", "(-oo; 4] U (23; +oo)")]
        [DataRow("[0; +oo)", "(-oo; 0)")]
        [DataRow("(2; 5) U (7; 9)", "(-oo; 2] U [5; 7] U [9; +oo)")]
        [DataRow("[2; 5] U [7; 9]", "(-oo; 2) U (5; 7) U (9; +oo)")]
        [DataRow("[1; 1]", "(-oo; 1) U (1; +oo)")]
        public void Complement(string setText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);

            originalSet.Complement();

            AssertEquals(originalSet, resultingSet);
        }

        private static void AssertEquals(DenseSet<int> a, DenseSet<int> b)
        {
            Assert.IsTrue(a.SequenceEqual(b));
            // Assert.IsTrue(a.SetEquals(b));
            // Assert.IsTrue(b.SetEquals(a));
            // Assert.IsTrue(a.IsSubsetOf(b));
            // Assert.IsTrue(b.IsSubsetOf(a));
            // Assert.IsFalse(a.IsProperSubsetOf(b));
            // Assert.IsFalse(b.IsProperSubsetOf(a));
            // Assert.IsTrue(a.IsSupersetOf(b));
            // Assert.IsTrue(b.IsSupersetOf(a));
            // Assert.IsFalse(a.IsProperSupersetOf(b));
            // Assert.IsFalse(b.IsProperSupersetOf(a));
        }

        private static DenseSet<int> ParseDenseSet(string text)
        {
            text = text.Trim();
            // Empty string means empty set
            if (text.Length == 0) return new();

            // Split by Union and parse intervals
            var intervalParts = text.Split('U');
            var intervals = intervalParts.Select(t => Interval<int>.Parse(t.Trim(), int.Parse));

            // Construct the dense set
            var result = new DenseSet<int>();
            foreach (var iv in intervals) result.Add(iv);

            // Check, if the constructed set is indeed the specified one
            Assert.IsTrue(intervals.SequenceEqual(result));

            return result;
        }
    }
}
