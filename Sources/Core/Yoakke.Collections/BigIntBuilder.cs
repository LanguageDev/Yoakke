// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    /// <summary>
    /// A builder type for <see cref="BigInt"/> for in-place modification.
    /// </summary>
    public class BigIntBuilder
    {
        private static readonly byte[] One = new byte[] { 1 };

        /// <summary>
        /// The bytes that get modified from the operations.
        /// </summary>
        public Memory<byte> Bytes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigIntBuilder"/> class.
        /// </summary>
        /// <param name="bytes">The bytes to modify in-place.</param>
        public BigIntBuilder(Memory<byte> bytes)
        {
            this.Bytes = bytes;
        }

        /// <summary>
        /// Bitwise negates the number.
        /// </summary>
        public void Invert()
        {
            for (var i = 0; i < this.Bytes.Length; ++i) this.Bytes.Span[i] = (byte)~this.Bytes.Span[i];
        }

        /// <summary>
        /// Adds bytes to this.
        /// </summary>
        /// <param name="other">The bytes to add.</param>
        /// <param name="overflow">True gets written here, if overflow happened.</param>
        public void Add(ReadOnlySpan<byte> other, out bool overflow)
        {
            var carry = 0;
            for (var i = 0; i < this.Bytes.Length; ++i)
            {
                unchecked
                {
                    var sum = this.Bytes.Span[i] + carry;
                    if (i < other.Length) sum += other[i];
                    carry = sum >= 256 ? 1 : 0;
                    this.Bytes.Span[i] = (byte)sum;
                }
            }
            overflow = carry == 1;
        }

        /// <summary>
        /// Performs 2s complement.
        /// </summary>
        public void TwosComplement()
        {
            this.Invert();
            this.Add(One, out _);
        }
    }
}
