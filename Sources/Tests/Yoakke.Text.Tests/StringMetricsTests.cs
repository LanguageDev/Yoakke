// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Text.Tests
{
    [TestClass]
    public class StringMetricsTests
    {
        [DataRow(3, "kitten", "sitting")]
        [DataRow(2, "abcd", "acbd")]
        [DataRow(3, "abc", "def")]
        [DataRow(4, "abcd", "defg")]
        [DataRow(2, "apple", "appel")]
        [DataRow(2, "mug", "gum")]
        [DataRow(3, "mcdonalds", "mcdnoald")]
        [DataRow(5, "table", "desk")]
        [DataRow(3, "abcd", "badc")]
        [DataRow(2, "book", "back")]
        [DataRow(4, "pattern", "parent")]
        [DataRow(4, "abcd", "")]
        [DataRow(0, "abcd", "abcd")]
        [DataRow(4, "abcd", "da")]
        [DataTestMethod]
        public void LevenshteinDistance(int expextedDistance, string s1, string s2) =>
            Assert.AreEqual(expextedDistance, Metrics.LevenshteinDistance.Instance.Distance(s1, s2));

        [DataRow(3, "kitten", "sitting")]
        [DataRow(1, "abcd", "acbd")]
        [DataRow(3, "abc", "def")]
        [DataRow(4, "abcd", "defg")]
        [DataRow(1, "apple", "appel")]
        [DataRow(2, "mug", "gum")]
        [DataRow(2, "mcdonalds", "mcdnoald")]
        [DataRow(5, "table", "desk")]
        [DataRow(2, "abcd", "badc")]
        [DataRow(2, "book", "back")]
        [DataRow(4, "abcd", "")]
        [DataRow(0, "abcd", "abcd")]
        [DataRow(4, "abcd", "da")]
        [DataTestMethod]
        public void OptimalStringAlignmentDistance(int expextedDistance, string s1, string s2) =>
            Assert.AreEqual(expextedDistance, Metrics.OptimalStringAlignmentDistance.Instance.Distance(s1, s2));
    }
}
