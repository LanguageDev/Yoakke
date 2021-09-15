// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Values;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class ValueDictionaryTests
    {
        [TestMethod]
        public void Equal()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();

            Assert.AreEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
            Assert.AreEqual(dict1.GetHashCode(), dict2.GetHashCode());
        }

        [TestMethod]
        public void EqualDifferentOrder()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "b", 2 }, { "c", 3 }, { "a", 1 } }.ToValue();

            Assert.AreEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
            Assert.AreEqual(dict1.GetHashCode(), dict2.GetHashCode());
        }

        [TestMethod]
        public void DifferentCount()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }

        [TestMethod]
        public void DifferentKey()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "d", 3 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }

        [TestMethod]
        public void DifferentValue()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 4 } }.ToValue();

            Assert.AreNotEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
        }
    }
}
