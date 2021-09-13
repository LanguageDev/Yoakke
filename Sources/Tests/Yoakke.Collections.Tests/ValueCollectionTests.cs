// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Values;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class ValueCollectionTests
    {
        [TestMethod]
        public void EqualReadOnlyValueLists()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 3 }.ToValue();

            Assert.AreEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
            Assert.AreEqual(list1.GetHashCode(), list2.GetHashCode());
        }

        [TestMethod]
        public void DifferentCountReadOnlyValueLists()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 3, 4 }.ToValue();

            Assert.AreNotEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
        }

        [TestMethod]
        public void DifferentValueReadOnlyValueLists()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 4 }.ToValue();

            Assert.AreNotEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
        }

        [TestMethod]
        public void EqualReadOnlyValueDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();

            Assert.AreEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
            Assert.AreEqual(dict1.GetHashCode(), dict2.GetHashCode());
        }

        [TestMethod]
        public void DifferentCountReadOnlyValueDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }

        [TestMethod]
        public void DifferentKeyReadOnlyValueDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "d", 3 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }

        [TestMethod]
        public void DifferentValueReadOnlyValueDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 4 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }
    }
}
