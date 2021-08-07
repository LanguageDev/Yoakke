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

        [DataRow(0, 1)]
        [DataRow(123, 1)]
        [DataRow(255, 1)]
        [DataRow(560, 1)]
        [DataRow(0, 2)]
        [DataRow(123, 2)]
        [DataRow(255, 2)]
        [DataRow(560, 2)]
        [DataTestMethod]
        public void ShiftTests(int number, int amount)
        {
            var bigInt = new BigInt(32, BitConverter.GetBytes(number));

            Assert.AreEqual((uint)(number << amount), (uint)(bigInt << amount));
            Assert.AreEqual((uint)(number >> amount), (uint)(bigInt >> amount));
        }

        [TestMethod]
        public void SignedArithmetic()
        {
            var ns = new int[]
            {
                -758373627, -971429535, -196760301, -308974598, 534580882, 74251856, -698399917,
                947149041, 548737911, -993708156, 713740392, -993670816, 893512852, 497526774,
                542371407, -972543078, -106116221, -165481548, 315734702, 396249911, 263831299,
                -295630055, -685275396, 881986713, -893219011, -989181881, -925563361, 226807330,
                -74527356, -730197948, 0,
            };
            foreach (var n1 in ns)
            {
                foreach (var n2 in ns)
                {
                    var b1 = new BigInt(32, BitConverter.GetBytes(n1));
                    var b2 = new BigInt(32, BitConverter.GetBytes(n2));

                    Assert.AreEqual(unchecked(n1 + n2), (int)(b1 + b2));
                    Assert.AreEqual(unchecked(n1 - n2), (int)(b1 - b2));
                    Assert.AreEqual(unchecked(n1 * n2), (int)BigInt.SignedMultiply(b1, b2));
                    if (n2 == 0)
                    {
                        Assert.ThrowsException<DivideByZeroException>(() => BigInt.SignedDivide(b1, b2));
                        Assert.ThrowsException<DivideByZeroException>(() => BigInt.SignedModulo(b1, b2));
                    }
                    else
                    {
                        Assert.AreEqual(unchecked(n1 / n2), (int)BigInt.SignedDivide(b1, b2));
                        // TODO: We need to define what modulo means for negatives
                        // Assert.AreEqual(unchecked(n1 % n2), (int)BigInt.SignedModulo(b1, b2));
                    }
                }
            }
        }

        [TestMethod]
        public void UnsignedArithmetic()
        {
            var ns = new int[]
            {
                -758373627, -971429535, -196760301, -308974598, 534580882, 74251856, -698399917,
                947149041, 548737911, -993708156, 713740392, -993670816, 893512852, 497526774,
                542371407, -972543078, -106116221, -165481548, 315734702, 396249911, 263831299,
                -295630055, -685275396, 881986713, -893219011, -989181881, -925563361, 226807330,
                -74527356, -730197948, 0,
            };
            foreach (var n1_ in ns)
            {
                foreach (var n2_ in ns)
                {
                    var n1 = unchecked((uint)n1_);
                    var n2 = unchecked((uint)n2_);

                    var b1 = new BigInt(32, BitConverter.GetBytes(n1));
                    var b2 = new BigInt(32, BitConverter.GetBytes(n2));

                    Assert.AreEqual(unchecked(n1 + n2), (uint)(b1 + b2));
                    Assert.AreEqual(unchecked(n1 - n2), (uint)(b1 - b2));
                    Assert.AreEqual(unchecked(n1 * n2), (uint)BigInt.UnsignedMultiply(b1, b2));
                    if (n2 == 0)
                    {
                        Assert.ThrowsException<DivideByZeroException>(() => BigInt.UnsignedDivide(b1, b2));
                        Assert.ThrowsException<DivideByZeroException>(() => BigInt.UnsignedModulo(b1, b2));
                    }
                    else
                    {
                        Assert.AreEqual(unchecked(n1 / n2), (uint)BigInt.UnsignedDivide(b1, b2));
                        Assert.AreEqual(unchecked(n1 % n2), (uint)BigInt.UnsignedModulo(b1, b2));
                    }
                }
            }
        }

        [TestMethod]
        public void SignedRelational()
        {
            var ns = new int[]
            {
                -758373627, -971429535, -196760301, -308974598, 534580882, 74251856, -698399917,
                947149041, 548737911, -993708156, 713740392, -993670816, 893512852, 497526774,
                542371407, -972543078, -106116221, -165481548, 315734702, 396249911, 263831299,
                -295630055, -685275396, 881986713, -893219011, -989181881, -925563361, 226807330,
                -74527356, -730197948, 0,
            };
            foreach (var n1 in ns)
            {
                foreach (var n2 in ns)
                {
                    var b1 = new BigInt(32, BitConverter.GetBytes(n1));
                    var b2 = new BigInt(32, BitConverter.GetBytes(n2));

                    Assert.AreEqual(n1 == n2, b1 == b2);
                    Assert.AreEqual(n1 != n2, b1 != b2);
                    Assert.AreEqual(n1 > n2, b1.SignedCompareTo(b2) > 0);
                    Assert.AreEqual(n1 < n2, b1.SignedCompareTo(b2) < 0);
                    Assert.AreEqual(n1 >= n2, b1.SignedCompareTo(b2) >= 0);
                    Assert.AreEqual(n1 <= n2, b1.SignedCompareTo(b2) <= 0);
                }
            }
        }

        [TestMethod]
        public void UnsignedRelational()
        {
            var ns = new int[]
            {
                -758373627, -971429535, -196760301, -308974598, 534580882, 74251856, -698399917,
                947149041, 548737911, -993708156, 713740392, -993670816, 893512852, 497526774,
                542371407, -972543078, -106116221, -165481548, 315734702, 396249911, 263831299,
                -295630055, -685275396, 881986713, -893219011, -989181881, -925563361, 226807330,
                -74527356, -730197948, 0,
            };
            foreach (var n1_ in ns)
            {
                foreach (var n2_ in ns)
                {
                    var n1 = unchecked((uint)n1_);
                    var n2 = unchecked((uint)n2_);

                    var b1 = new BigInt(32, BitConverter.GetBytes(n1));
                    var b2 = new BigInt(32, BitConverter.GetBytes(n2));

                    Assert.AreEqual(n1 == n2, b1 == b2);
                    Assert.AreEqual(n1 != n2, b1 != b2);
                    Assert.AreEqual(n1 > n2, b1.UnsignedCompareTo(b2) > 0);
                    Assert.AreEqual(n1 < n2, b1.UnsignedCompareTo(b2) < 0);
                    Assert.AreEqual(n1 >= n2, b1.UnsignedCompareTo(b2) >= 0);
                    Assert.AreEqual(n1 <= n2, b1.UnsignedCompareTo(b2) <= 0);
                }
            }
        }
    }
}
