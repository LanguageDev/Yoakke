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
        [DataRow(
            "",
            "[2; 3) => { 1 }",
            "[2; 3) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[2; 3) => { 2 }",
            "[2; 3) => { 2 } U [5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[2; 5) => { 2 }",
            "[2; 5) => { 2 } U [5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[9; 11) => { 2 }",
            "[5; 7) => { 1 } U [9; 11) => { 2 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[7; 12) => { 2 }",
            "[5; 7) => { 1 } U [7; 12) => { 2 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[17; 19) => { 2 }",
            "[5; 7) => { 1 } U [12; 15) => { 1 } U [17; 19) => { 2 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[15; 19) => { 2 }",
            "[5; 7) => { 1 } U [12; 15) => { 1 } U [15; 19) => { 2 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[5; 7) => { 2 }",
            "[5; 7) => { 1, 2 }")]
        [DataRow(
            "[3; 9) => { 1 }",
            "[5; 7) => { 2 }",
            "[3; 5) => { 1 } U [5; 7) => { 1, 2 } U [7; 9) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[3; 9) => { 2 }",
            "[3; 5) => { 2 } U [5; 7) => { 1, 2 } U [7; 9) => { 2 }")]
        [DataRow(
            "[5; 9) => { 1 }",
            "[2; 7) => { 2 }",
            "[2; 5) => { 2 } U [5; 7) => { 1, 2 } U [7; 9) => { 1 }")]
        [DataRow(
            "[5; 9) => { 1 }",
            "[7; 12) => { 2 }",
            "[5; 7) => { 1 } U [7; 9) => { 1, 2 } U [9; 12) => { 2 }")]
        [DataRow(
            "[5; 11) => { 1 }",
            "[5; 7) => { 2 }",
            "[5; 7) => { 1, 2 } U [7; 11) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[5; 11) => { 2 }",
            "[5; 7) => { 1, 2 } U [7; 11) => { 2 }")]
        [DataRow(
            "[5; 11) => { 1 }",
            "[8; 11) => { 2 }",
            "[5; 8) => { 1 } U [8; 11) => { 1, 2 }")]
        [DataRow(
            "[8; 11) => { 1 }",
            "[5; 11) => { 2 }",
            "[5; 8) => { 2 } U [8; 11) => { 1, 2 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 }",
            "[5; 17) => { 2 }",
            "[1; 3) => { 1 } U [5; 7) => { 1, 2 } U [7; 9) => { 2 } U [9; 12) => { 1, 2 } U [12; 14) => { 2 } U [14; 15) => { 1, 2 } U [15; 17) => { 1, 2 } U [18; 19) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 } U [21; 24) => { 1 }",
            "[5; 19) => { 2 }",
            "[1; 3) => { 1 } U [5; 7) => { 1, 2 } U [7; 9) => { 2 } U [9; 12) => { 1, 2 } U [12; 14) => { 2 } U [14; 15) => { 1, 2 } U [15; 17) => { 1, 2 } U [17; 18) => { 2 } U [18; 19) => { 1, 2 } U [21; 24) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [7; 9) => { 1 } U [9; 12) => { 1 } U [12; 14) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 }",
            "[5; 17) => { 2 }",
            "[1; 3) => { 1 } U [5; 7) => { 1, 2 } U [7; 9) => { 1, 2 } U [9; 12) => { 1, 2 } U [12; 14) => { 1, 2 } U [14; 15) => { 1, 2 } U [15; 17) => { 1, 2 } U [18; 19) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[5; 10) => { 2 }",
            "[1; 3) => { 1 } U [5; 7) => { 1, 2 } U [7; 9) => { 2 } U [9; 10) => { 1, 2 } U [10; 12) => { 1 } U [14; 15) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[6; 12) => { 2 }",
            "[1; 3) => { 1 } U [5; 6) => { 1 } U [6; 7) => { 1, 2 } U [7; 9) => { 2 } U [9; 12) => { 1, 2 } U [14; 15) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[6; 10) => { 2 }",
            "[1; 3) => { 1 } U [5; 6) => { 1 } U [6; 7) => { 1, 2 } U [7; 9) => { 2 } U [9; 10) => { 1, 2 } U [10; 12) => { 1 } U [14; 15) => { 1 }")]
        public void AddInterval(string mapText, string assocText, string resultText)
        {
            var originalMap = ParseDenseMap(mapText);
            var resultingMap = ParseDenseMap(resultText);
            var assoc = ParseAssociation(assocText);

            originalMap.Add(assoc.Key, assoc.Value);

            AssertEquals(originalMap, resultingMap);
        }

        [DataTestMethod]
        [DataRow(
            "[2; 5) => { 1 } U [7; 9) => { 1 }",
            "[4; 8)",
            "[2; 4) => { 1 } U [8; 9) => { 1 }")]
        [DataRow(
            "[2; 5) => { 1 } U [7; 9) => { 1 }",
            "[4; 9)",
            "[2; 4) => { 1 }")]
        [DataRow(
            "[2; 5) => { 1 } U [7; 9) => { 1 }",
            "[2; 8)",
            "[8; 9) => { 1 }")]
        [DataRow(
            "(-oo; +oo) => { 1 }",
            "[0; 0]",
            "(-oo; 0) => { 1 } U (0; +oo) => { 1 }")]
        // Legacy tests inverted
        [DataRow(
            "",
            "[2; 3)",
            "")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[2; 3)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[2; 5)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[9; 11)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[7; 12)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[17; 19)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 } U [12; 15) => { 1 }",
            "[15; 19)",
            "[5; 7) => { 1 } U [12; 15) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[5; 7)",
            "")]
        [DataRow(
            "[3; 9) => { 1 }",
            "[5; 7)",
            "[3; 5) => { 1 } U [7; 9) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[3; 9)",
            "")]
        [DataRow(
            "[5; 9) => { 1 }",
            "[2; 7)",
            "[7; 9) => { 1 }")]
        [DataRow(
            "[5; 9) => { 1 }",
            "[7; 12)",
            "[5; 7) => { 1 }")]
        [DataRow(
            "[5; 11) => { 1 }",
            "[5; 7)",
            "[7; 11) => { 1 }")]
        [DataRow(
            "[5; 7) => { 1 }",
            "[5; 11)",
            "")]
        [DataRow(
            "[5; 11) => { 1 }",
            "[8; 11)",
            "[5; 8) => { 1 }")]
        [DataRow(
            "[8; 11) => { 1 }",
            "[5; 11)",
            "")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 }",
            "[5; 17)",
            "[1; 3) => { 1 } U [18; 19) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 } U [21; 24) => { 1 }",
            "[5; 19)",
            "[1; 3) => { 1 } U [21; 24) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [7; 9) => { 1 } U [9; 12) => { 1 } U [12; 14) => { 1 } U [14; 15) => { 1 } U [15; 17) => { 1 } U [18; 19) => { 1 }",
            "[5; 17)",
            "[1; 3) => { 1 } U [18; 19) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[5; 10)",
            "[1; 3) => { 1 } U [10; 12) => { 1 } U [14; 15) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[6; 12)",
            "[1; 3) => { 1 } U [5; 6) => { 1 } U [14; 15) => { 1 }")]
        [DataRow(
            "[1; 3) => { 1 } U [5; 7) => { 1 } U [9; 12) => { 1 } U [14; 15) => { 1 }",
            "[6; 10)",
            "[1; 3) => { 1 } U [5; 6) => { 1 } U [10; 12) => { 1 } U [14; 15) => { 1 }")]
        public void RemoveInterval(string mapText, string intervalText, string resultText)
        {
            var originalMap = ParseDenseMap(mapText);
            var resultingMap = ParseDenseMap(resultText);
            var interval = ParseInterval(intervalText);

            originalMap.Remove(interval);

            AssertEquals(originalMap, resultingMap);
        }

        [DataTestMethod]
        [DataRow("", "(0; 0)", true)]
        [DataRow("[0; 5) => { 1 }", "[6; 7)", false)]
        [DataRow("[0; 5) => { 1 }", "[0; 5)", true)]
        [DataRow("[0; 5) => { 1 }", "[0; 1)", true)]
        [DataRow("[0; 5) => { 1 }", "[4; 5)", true)]
        [DataRow("[0; 5) => { 1 }", "[2; 3)", true)]
        [DataRow("(-oo; +oo) => { 1 }", "[2; 3)", true)]
        [DataRow("[0; 1) => { 1 } U [1; 2) => { 1 }", "[0; 2)", true)]
        [DataRow("[0; 1) => { 1 } U [1; 2) => { 1 }", "[0; 3)", false)]
        [DataRow("[0; 1) => { 1 } U [1; 2) => { 1 }", "[-1; 2)", false)]
        [DataRow("[0; 1) => { 1 } U [1; 2) => { 1 }", "[-1; 3)", false)]
        [DataRow("[0; 1) => { 1 } U [2; 3) => { 1 }", "[1; 2)", false)]
        [DataRow("[0; 1) => { 1 } U [2; 3) => { 1 }", "[0; 3)", false)]
        public void ContainsInterval(string mapText, string intervalText, bool contains)
        {
            var map = ParseDenseMap(mapText);
            var interval = ParseInterval(intervalText);

            var result = map.ContainsKeys(interval);

            Assert.AreEqual(contains, result);
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
