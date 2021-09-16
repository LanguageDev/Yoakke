// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Values;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class ValueListTests
    {
        [TestMethod]
        public void Equal()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 3 }.ToValue();

            Assert.AreEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
            Assert.AreEqual(list1.GetHashCode(), list2.GetHashCode());
        }

        [TestMethod]
        public void DifferentCount()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 3, 4 }.ToValue();

            Assert.AreNotEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
        }

        [TestMethod]
        public void DifferentValue()
        {
            var list1 = new List<int> { 1, 2, 3 }.ToValue();
            var list2 = new List<int> { 1, 2, 4 }.ToValue();

            Assert.AreNotEqual(list1, list2);
            Assert.IsFalse(ReferenceEquals(list1, list2));
        }
    }
}
