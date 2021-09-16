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
        [DataRow("(-oo; 0) U (0; +oo)", "0", "(-oo; +oo)")]
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
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "8", "(-oo; 5] U (7; 8) U (8; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "14", "(-oo; 5] U (7; 9) U [12; 14) U (14; 16]")]
        [DataRow("(-oo; +oo)", "0", "(-oo; 0) U (0; +oo)")]
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
        // Legacy tests
        [DataRow("", "[2; 3)", "[2; 3)")]
        [DataRow("[5; 7) U [12; 15)", "[2; 3)", "[2; 3) U [5; 7) U [12; 15)")]
        [DataRow("[5; 7) U [12; 15)", "[17; 19)", "[5; 7) U [12; 15) U [17; 19)")]
        [DataRow("[5; 7) U [12; 15)", "[8; 11)", "[5; 7) U [8; 11) U [12; 15)")]
        [DataRow("[5; 7) U [12; 15)", "[3; 5)", "[3; 7) U [12; 15)")]
        [DataRow("[5; 7) U [12; 15)", "[3; 6)", "[3; 7) U [12; 15)")]
        [DataRow("[5; 7) U [12; 15)", "[7; 12)", "[5; 15)")]
        [DataRow("[5; 7) U [12; 15)", "[3; 16)", "[3; 16)")]
        public void AddInterval(string setText, string intervalText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var interval = ParseInterval(intervalText);

            originalSet.Add(interval);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("", "(2; 4]", "")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", "(-oo; 1) U (1; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", "(-oo; 5] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(3; 5]", "(-oo; 3] U (7; 9) U [12; 16]")]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(14; 15]", "(-oo; 5] U (7; 9) U [12; 14] U (15; 16]")]
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
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-2; 24]", "(-oo; -2]")]
        [DataRow("(-oo; +oo)", "[0; 3)", "(-oo; 0) U [3; +oo)")]
        public void RemoveInterval(string setText, string intervalText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);
            var interval = ParseInterval(intervalText);

            originalSet.Remove(interval);

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)")]
        [DataRow("(4; 23]", "(-oo; 4] U (23; +oo)")]
        [DataRow("[0; +oo)", "(-oo; 0)")]
        [DataRow("(2; 5) U (7; 9)", "(-oo; 2] U [5; 7] U [9; +oo)")]
        [DataRow("[2; 5] U [7; 9]", "(-oo; 2) U (5; 7) U (9; +oo)")]
        [DataRow("[1; 1]", "(-oo; 1) U (1; +oo)")]
        // Legacy tests
        [DataRow("", "(-oo; +oo)")]
        [DataRow("(-oo; +oo)", "")]
        [DataRow("[3; 5)", "(-oo; 3) U [5; +oo)")]
        [DataRow("[3; 5) U [7; 9)", "(-oo; 3) U [5; 7) U [9; +oo)")]
        [DataRow("[3; 5) U [7; 8) U [10; 14) U [18; 21)", "(-oo; 3) U [5; 7) U [8; 10) U [14; 18) U [21; +oo)")]
        [DataRow("[3; 5) U [7; 8) U [10; 14) U [18; 21) U [24; 26)", "(-oo; 3) U [5; 7) U [8; 10) U [14; 18) U [21; 24) U [26; +oo)")]
        [DataRow("(-oo; 5)", "[5; +oo)")]
        [DataRow("[5; +oo)", "(-oo; 5)")]
        [DataRow("[2; 4) U [6; 7) U [8; 11) U [14; +oo)", "(-oo; 2) U [4; 6) U [7; 8) U [11; 14)")]
        [DataRow("[2; 4) U [6; 7) U [8; 11) U [14; 16) U [18; +oo)", "(-oo; 2) U [4; 6) U [7; 8) U [11; 14) U [16; 18)")]
        [DataRow("(-oo; 2) U [6; 7) U [8; 11) U [14; 16)", "[2; 6) U [7; 8) U [11; 14) U [16; +oo)")]
        [DataRow("(-oo; 2) U [6; 7) U [8; 11) U [14; 16) U [18; 21)", "[2; 6) U [7; 8) U [11; 14) U [16; 18) U [21; +oo)")]
        public void Complement(string setText, string resultText)
        {
            var originalSet = ParseDenseSet(setText);
            var resultingSet = ParseDenseSet(resultText);

            originalSet.Complement();

            AssertEquals(originalSet, resultingSet);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "4", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "12", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "7", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "6", false)]
        [DataRow("(-oo; +oo)", "0", true)]
        [DataRow("(-oo; 0) U (0; +oo)", "0", false)]
        public void ContainsItem(string setText, string itemText, bool contained)
        {
            var set = ParseDenseSet(setText);
            var item = int.Parse(itemText);

            var result = set.Contains(item);

            if (contained) Assert.IsTrue(result);
            else Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[2; 5]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[5; 5]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(13; 15]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", true)]
        [DataRow("(-oo; +oo)", "[-123; 54]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[2; 6]", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(7; 16]", false)]
        public void ContainsInterval(string setText, string intervalText, bool contained)
        {
            var set = ParseDenseSet(setText);
            var interval = ParseInterval(intervalText);

            var result = set.Contains(interval);

            if (contained) Assert.IsTrue(result);
            else Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9)", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("", "", true)]
        [DataRow("[12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("[16; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", true)]
        [DataRow("(-oo; +oo)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("[3; 7) U [9; 13)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("[9; 9]", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("(5; 7] U [9; 12)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        public void IsSubset(string set1Text, string set2Text, bool isSubset)
        {
            var set1 = ParseDenseSet(set1Text);
            var set2 = ParseDenseSet(set2Text);

            if (isSubset)
            {
                Assert.IsTrue(set1.IsSubsetOf(set2));
                Assert.IsTrue(set2.IsSupersetOf(set1));
            }
            else
            {
                Assert.IsFalse(set1.IsSubsetOf(set2));
                Assert.IsFalse(set2.IsSupersetOf(set1));
            }
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9)", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("", "", false)]
        [DataRow("[12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("(12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("[16; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", true)]
        [DataRow("(-oo; +oo)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("[3; 7) U [9; 13)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("[9; 9]", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        [DataRow("(5; 7] U [9; 12)", "(-oo; 5] U (7; 9) U [12; 16]", false)]
        public void IsProperSubset(string set1Text, string set2Text, bool isSubset)
        {
            var set1 = ParseDenseSet(set1Text);
            var set2 = ParseDenseSet(set2Text);

            if (isSubset)
            {
                // Non-proper
                Assert.IsTrue(set1.IsSubsetOf(set2));
                Assert.IsTrue(set2.IsSupersetOf(set1));
                // Proper
                Assert.IsTrue(set1.IsProperSubsetOf(set2));
                Assert.IsTrue(set2.IsProperSupersetOf(set1));
            }
            else
            {
                Assert.IsFalse(set1.IsProperSubsetOf(set2));
                Assert.IsFalse(set2.IsProperSupersetOf(set1));
            }
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[2; 7)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[13; 14)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[9; 12)", false)]
        [DataRow("", "(0; 0)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", false)]
        [DataRow("", "(2; 6)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[2; 20)", true)]
        [DataRow("[5; 5]", "[5; 5]", true)]
        [DataRow("[5; 5]", "[1; 1]", false)]
        public void OverlapsInterval(string setText, string intervalText, bool overlaps)
        {
            var set = ParseDenseSet(setText);
            var interval = ParseInterval(intervalText);

            var result = set.Overlaps(interval);

            if (overlaps) Assert.IsTrue(result);
            else Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 4) U (4; +oo)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[5; 7) U [18; 20)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(6; 7) U [18; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 7) U [18; 20)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [18; 20)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [13; 20)", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", true)]
        public void OverlapsSet(string set1Text, string set2Text, bool hasOverlap)
        {
            var set1 = ParseDenseSet(set1Text);
            var set2 = ParseDenseSet(set2Text);

            if (hasOverlap)
            {
                Assert.IsTrue(set1.Overlaps(set2));
                Assert.IsTrue(set2.Overlaps(set1));
            }
            else
            {
                Assert.IsFalse(set1.Overlaps(set2));
                Assert.IsFalse(set2.Overlaps(set1));
            }
        }

        [DataTestMethod]
        [DataRow("", "", true)]
        [DataRow("[1; 1]", "[1; 1]", true)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]", true)]
        [DataRow("(1; 2) U (3; 4)", "(1; 2) U (3; 4)", true)]
        [DataRow("[1; 2] U (3; 4)", "[1; 2] U (3; 4)", true)]
        [DataRow("[1; 2] U (3; 4)", "(1; 2] U (3; 4)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9) U [12; 16]", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U [12; 16]", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[5; 7) U [18; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "(6; 7) U [18; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 7) U [18; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [18; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [13; 20)", false)]
        [DataRow("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", false)]
        public void EqualSet(string set1Text, string set2Text, bool equal)
        {
            var set1 = ParseDenseSet(set1Text);
            var set2 = ParseDenseSet(set2Text);

            if (equal)
            {
                AssertEquals(set1, set2);
            }
            else
            {
                Assert.IsFalse(set1.SetEquals(set2));
                Assert.IsFalse(set2.SetEquals(set1));
            }
        }

        private static void AssertEquals(DenseSet<int> a, DenseSet<int> b)
        {
            // Same intervals
            Assert.IsTrue(a.SequenceEqual(b));
            // Set-equality
            Assert.IsTrue(a.SetEquals(b));
            Assert.IsTrue(b.SetEquals(a));
            // Subset-ness
            Assert.IsTrue(a.IsSubsetOf(b));
            Assert.IsTrue(b.IsSubsetOf(a));
            // Proper subset-ness
            Assert.IsFalse(a.IsProperSubsetOf(b));
            Assert.IsFalse(b.IsProperSubsetOf(a));
            // Superset-ness
            Assert.IsTrue(a.IsSupersetOf(b));
            Assert.IsTrue(b.IsSupersetOf(a));
            // Proper superset-ness
            Assert.IsFalse(a.IsProperSupersetOf(b));
            Assert.IsFalse(b.IsProperSupersetOf(a));
        }

        private static DenseSet<int> ParseDenseSet(string text)
        {
            text = text.Trim();
            // Empty string means empty set
            if (text.Length == 0) return new();

            // Split by Union and parse intervals
            var intervalParts = text.Split('U');
            var intervals = intervalParts.Select(ParseInterval);

            // Construct the dense set
            var result = new DenseSet<int>();
            foreach (var iv in intervals) result.Add(iv);

            // Check, if the constructed set is indeed the specified one
            Assert.IsTrue(intervals.SequenceEqual(result));

            return result;
        }

        private static Interval<int> ParseInterval(string text) => Interval<int>.Parse(text.Trim(), int.Parse);
    }
}
