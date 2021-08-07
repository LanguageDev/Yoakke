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
        private static IEnumerable<object[]> BigIntegerConversionInputs { get; } = new object[][]
        {
            /*           width    big integer       bytes          */
            new object[] { 8, new BigInteger(0), new byte[] { 0 } },
            new object[] { 8, new BigInteger(1), new byte[] { 1 } },
            new object[] { 8, new BigInteger(255), new byte[] { 0b11111111 } },
            new object[] { 8, new BigInteger(127), new byte[] { 0b01111111 } },
            new object[] { 8, new BigInteger(-128), new byte[] { 0b10000000 } },
            new object[] { 8, new BigInteger(-1), new byte[] { 0b11111111 } },
            new object[] { 16, new BigInteger(256), new byte[] { 0, 1 } },
            new object[] { 16, new BigInteger(-256), new byte[] { 0, 0b11111111 } },
            new object[] { 10, new BigInteger(256), new byte[] { 0, 1 } },
            new object[] { 10, new BigInteger(-256), new byte[] { 0, 0b11 } },
        };

        [DynamicData(nameof(BigIntegerConversionInputs))]
        [DataTestMethod]
        public void BigIntegerConversionTests(int width, BigInteger bigInteger, byte[] expectedBytes)
        {
            var bigInt = BigInt.FromBigInteger(width, bigInteger);
            var bigIntegerBack = bigInt.ToBigInteger(bigInteger < 0);

            // Exact byte
            Assert.AreEqual(expectedBytes.Length, bigInt.Bytes.Length);
            Assert.IsTrue(bigInt.Bytes.Span.SequenceEqual(expectedBytes));
            // Back conversion
            Assert.AreEqual(bigInteger, bigIntegerBack);
        }

        [DataRow(5)]
        [DataRow(8)]
        [DataRow(10)]
        [DataRow(15)]
        [DataRow(16)]
        [DataRow(17)]
        [DataRow(20)]
        [DataRow(32)]
        [DataRow(35)]
        [DataTestMethod]
        public void MinMaxValuesTests(int width)
        {
            // Unsigned minimum and maximum
            var umin = new BigInteger(0);
            var umax = (new BigInteger(1) << width) - 1;

            // Signed minimum and maximum
            var smin = -(new BigInteger(1) << (width - 1));
            var smax = (new BigInteger(1) << (width - 1)) - 1;

            // Assert them
            Assert.AreEqual(BigInt.MinValue(false, width), BigInt.FromBigInteger(width, umin));
            Assert.AreEqual(BigInt.MinValue(true, width), BigInt.FromBigInteger(width, smin));
            Assert.AreEqual(BigInt.MaxValue(false, width), BigInt.FromBigInteger(width, umax));
            Assert.AreEqual(BigInt.MaxValue(true, width), BigInt.FromBigInteger(width, smax));
        }
    }
}
