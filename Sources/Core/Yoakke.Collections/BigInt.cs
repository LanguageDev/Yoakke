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
    public readonly struct BigInt : IComparable<BigInt>, IEquatable<BigInt>
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
        /// Creates a <see cref="BigInt"/> from a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="signed">True, if the resulting <see cref="BigInt"/> should be signed.</param>
        /// <param name="width">The width of the resulting <see cref="BigInt"/> in bits.</param>
        /// <param name="bigInteger">The <see cref="BigInteger"/> to convert.</param>
        /// <returns>A new <see cref="BigInt"/> with equivalent value to <paramref name="bigInteger"/>.</returns>
        public static BigInt FromBigInteger(bool signed, int width, BigInteger bigInteger)
        {
            var negate = false;
            if (bigInteger.Sign < 0)
            {
                bigInteger = BigInteger.Abs(bigInteger);
                negate = true;
            }

            var builder = BigIntBuilder.WithBitCapacity(width);
            bigInteger.TryWriteBytes(builder.Bytes.Span, out _, true);
            if (negate) builder.TwosComplement();
            builder.MaskToWidth(width);

            return new(signed, width, builder);
        }

        /// <summary>
        /// Converts this <see cref="BigInt"/> to a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="signed">True, if the result should be interpreted as signed.</param>
        /// <returns>A <see cref="BigInteger"/> equivalent to this number.</returns>
        public BigInteger ToBigInteger(bool signed) => new(this.builder.Bytes.Span, !signed);

        /// <summary>
        /// Returns the smallest <see cref="BigInt"/> possible with the given width.
        /// </summary>
        /// <param name="signed">True, if should be signed.</param>
        /// <param name="width">The width of the requested <see cref="BigInt"/>.</param>
        /// <returns>The smallest <see cref="BigInt"/> possible with the given width and signedness.</returns>
        public static BigInt MinValue(bool signed, int width)
        {
            var builder = BigIntBuilder.WithBitCapacity(width);
            if (signed) builder[width - 1] = true;
            return new(signed, width, builder);
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

        /// <summary>
        /// Bitwise negates the given <see cref="BigInt"/>.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>A bitwise negation of <paramref name="bigInt"/>.</returns>
        public static BigInt operator ~(BigInt bigInt)
        {
            var builder = bigInt.builder.Clone();
            builder.BitwiseNegate();
            builder.MaskToWidth(bigInt.Width);
            return new(bigInt.IsSigned, bigInt.Width, builder);
        }

        /// <summary>
        /// Negates a <see cref="BigInt"/> using twos-complement.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>The negated <paramref name="bigInt"/>.</returns>
        public static BigInt operator -(BigInt bigInt)
        {
            var builder = bigInt.builder.Clone();
            builder.TwosComplement();
            builder.MaskToWidth(bigInt.Width);
            return new(bigInt.IsSigned, bigInt.Width, builder);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BigInt other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(BigInt other) => this.CompareTo(other) == 0;

        /// <inheritdoc/>
        public int CompareTo(BigInt other)
        {
            var cmp = BigIntBuilder.Compare(this.builder.Bytes.Span, other.builder.Bytes.Span);
            if (this.IsSigned != other.IsSigned) cmp = -cmp;
            return cmp;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // NOTE: For simplicity we just sum up the bytes
            // This is because BigInts with differnet widths can be equal
            // So we need an operation that doesn't yield different results for different
            // widths, only considers nonzero bytes
            var code = 0;
            for (var i = 0; i < this.builder.Bytes.Length; ++i)
            {
                unchecked
                {
                    code += this.builder.Bytes.Span[i];
                }
            }
            return code;
        }

        /// <summary>
        /// Compares two <see cref="BigInt"/>s for equality.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
        public static bool operator ==(BigInt left, BigInt right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="BigInt"/>s for inequality.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
        public static bool operator !=(BigInt left, BigInt right) => !(left == right);

        /// <summary>
        /// Less-than compares two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        public static bool operator <(BigInt left, BigInt right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Less-than or equal compares two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        public static bool operator <=(BigInt left, BigInt right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Greater-than compares two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        public static bool operator >(BigInt left, BigInt right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Greater-than or equal compares two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to compare.</param>
        /// <param name="right">The second <see cref="BigInt"/> to compare.</param>
        /// <returns>True, if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        public static bool operator >=(BigInt left, BigInt right) => left.CompareTo(right) >= 0;
    }
}
