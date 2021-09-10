// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class ValueCollectionTests
    {
        [TestMethod]
        public void TestReadOnlyValueDictionary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.AsValue();
            var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.AsValue();

            Assert.AreEqual(dict1, dict2);
            Assert.IsFalse(ReferenceEquals(dict1, dict2));
            Assert.AreEqual(dict1.GetHashCode(), dict2.GetHashCode());
        }
    }
}
