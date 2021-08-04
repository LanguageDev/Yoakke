// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    /// <summary>
    /// A fixed-width implementation of <see cref="BigInteger"/> with twos-complement representation.
    /// </summary>
    public readonly struct BigInt
    {
        private readonly BigIntBuilder builder;

        /// <summary>
        /// True, if the <see cref="BigInt"/> should be considered signed.
        /// </summary>
        public readonly bool IsSigned;

        /// <summary>
        /// The width of the <see cref="BigInt"/> in bits.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The bytes this <see cref="BigInt"/> consists of.
        /// </summary>
        public ReadOnlyMemory<byte> Bytes => this.builder.Bytes;

        /// <summary>
        /// True, if this number is even.
        /// </summary>
        public bool IsEven => this.builder.IsEven;

        /// <summary>
        /// True, if this number is odd.
        /// </summary>
        public bool IsOdd => this.builder.IsOdd;

        /// <summary>
        /// True, if this number is 0.
        /// </summary>
        public bool IsZero => this.builder.IsZero;

        /// <summary>
        /// The sign bit (MSB). True, if set to 1.
        /// </summary>
        public bool SignBit => this[this.Width - 1];

        /// <summary>
        /// Accesses a single bit of this number.
        /// </summary>
        /// <param name="index">The bit index.</param>
        /// <returns>True, if the bit at <paramref name="index"/> is set to 1.</returns>
        public bool this[int index] => this.builder[index];

        private BigInt(bool signed, int width, BigIntBuilder builder)
        {
            this.IsSigned = signed;
            this.Width = width;
            this.builder = builder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInt"/> struct.
        /// </summary>
        /// <param name="signed">True, if the number should be considered signed.</param>
        /// <param name="width">The width of the number in bits.</param>
        /// <param name="bytes">The initial bytes to assign.</param>
        public BigInt(bool signed, int width, ReadOnlySpan<byte> bytes)
        {
            var bytesLen = (width + 7) / 8;
            var resultBytes = new byte[bytesLen];
            bytes.TryCopyTo(resultBytes);

            this.IsSigned = signed;
            this.Width = width;
            this.builder = new(resultBytes);
            this.builder.MaskToWidth(width);
        }

        /// <summary>
        /// Returns the largest <see cref="BigInt"/> possible with the given width.
        /// </summary>
        /// <param name="signed">True, if should be signed.</param>
        /// <param name="width">The width of the requested <see cref="BigInt"/>.</param>
        /// <returns>The largest <see cref="BigInt"/> possible with the given width and signedness.</returns>
        public static BigInt MaxValue(bool signed, int width)
        {
            var builder = BigIntBuilder.WithBitCapacity(width);
            builder.SetAllBitsOne();
            builder.MaskToWidth(width);
            if (signed) builder[width - 1] = false;
            return new(signed, width, builder);
        }
    }
}
