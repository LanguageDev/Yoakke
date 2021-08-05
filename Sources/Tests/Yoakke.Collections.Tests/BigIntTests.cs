// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class BigIntTests
    {
        private static IEnumerable<object[]> FromBigIntegerInputs { get; } = new object[][]
        {
            /*           width    big integer       bytes          */
            new object[] { 8, new BigInteger(0), new byte[] { 0 } },
            new object[] { 8, new BigInteger(1), new byte[] { 1 } },
            new object[] { 8, new BigInteger(255), new byte[] { 0b11111111 } },
            new object[] { 8, new BigInteger(127), new byte[] { 0b01111111 } },
            new object[] { 8, new BigInteger(-128), new byte[] { 0b10000000 } },
            new object[] { 8, new BigInteger(-1), new byte[] { 0b11111111 } },
        };

        [DynamicData(nameof(FromBigIntegerInputs))]
        [DataTestMethod]
        public void FromBigIntegerTests(int width, BigInteger bigInteger, byte[] expectedBytes)
        {
            var bigInt = BigInt.FromBigInteger(false, width, bigInteger);

            Assert.AreEqual(expectedBytes.Length, bigInt.Bytes.Length);
            Assert.IsTrue(bigInt.Bytes.Span.SequenceEqual(expectedBytes));
        }
    }
}
