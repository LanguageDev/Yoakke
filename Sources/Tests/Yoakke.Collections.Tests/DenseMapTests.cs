// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class DenseMapTests
    {
        private static readonly ICombiner<HashSet<int>> HashSetCombiner =
            Combiner<HashSet<int>>.Create((s1, s2) => EnumerableExtensions.ToHashSet(s1.Concat(s2)));

        [DataTestMethod]
        // Legacy tests
        [DataRow("", "[2; 3) => { 1 }", "[2; 3) => { 1 }")]
        public void AddInterval(string mapText, string assocText, string resultText)
        {
            var originalMap = ParseDenseMap(mapText);
            var resultingMap = ParseDenseMap(resultText);
            var assoc = ParseAssociation(assocText);

            originalMap.Add(assoc.Key, assoc.Value);

            AssertEquals(originalMap, resultingMap);
        }

        private static void AssertEquals(DenseMap<int, HashSet<int>> a, IEnumerable<KeyValuePair<Interval<int>, HashSet<int>>> b)
        {
            // Key equality
            Assert.IsTrue(a.Select(i => i.Key).SequenceEqual(b.Select(i => i.Key)));
            // Value equality
            Assert.IsTrue(a.Select(i => i.Value).Zip(b.Select(i => i.Value)).All(pair => pair.First.SetEquals(pair.Second)));
        }

        private static DenseMap<int, HashSet<int>> ParseDenseMap(string text)
        {
            text = text.Trim();
            // Empty string means empty set
            if (text.Length == 0) return new();

            // Split by Union and parse intervals
            var intervalParts = text.Split('U');
            var intervalSetPairs = intervalParts.Select(ParseAssociation);

            // Construct the dense set
            var result = new DenseMap<int, HashSet<int>>(HashSetCombiner);
            foreach (var (iv, set) in intervalSetPairs) result.Add(iv, set);

            // Check equality
            AssertEquals(result, intervalSetPairs);

            return result;
        }

        private static KeyValuePair<Interval<int>, HashSet<int>> ParseAssociation(string text)
        {
            var parts = text.Split("=>");
            var interval = ParseInterval(parts[0]);
            var set = ParseSet(parts[1]);
            return new(interval, set);
        }

        private static HashSet<int> ParseSet(string text)
        {
            text = text.Trim();
            Assert.AreEqual('{', text[0]);
            Assert.AreEqual('}', text[^1]);
            return EnumerableExtensions.ToHashSet(text[1..^1].Split(',').Select(t => int.Parse(t.Trim())));
        }

        private static Interval<int> ParseInterval(string text) => Interval<int>.Parse(text.Trim(), int.Parse);
    }
}
