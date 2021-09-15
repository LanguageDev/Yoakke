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
