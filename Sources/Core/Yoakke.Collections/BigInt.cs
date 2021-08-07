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
    public readonly struct BigInt : IEquatable<BigInt>
    {
        private static readonly byte[] TopFillMasks = new byte[]
        {
            0b00000000, 0b10000000, 0b11000000, 0b11100000,
            0b11110000, 0b11111000, 0b11111100, 0b11111110,
        };

        private readonly BigIntBuilder builder;

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

        private BigInt(int width, BigIntBuilder builder)
        {
            this.Width = width;
            this.builder = builder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInt"/> struct.
        /// </summary>
        /// <param name="width">The width of the number in bits.</param>
        /// <param name="bytes">The initial bytes to assign.</param>
        public BigInt(int width, ReadOnlySpan<byte> bytes)
        {
            var bytesLen = (width + 7) / 8;
            var resultBytes = new byte[bytesLen];
            bytes.TryCopyTo(resultBytes);

            this.Width = width;
            this.builder = new(resultBytes);
            this.builder.MaskToWidth(width);
        }

        /// <summary>
        /// Attempts to write <see cref="Bytes"/> to a <see cref="Span{Byte}"/>.
        /// </summary>
        /// <param name="destination">The <see cref="Span{Byte}"/> to write to.</param>
        /// <param name="bytesWritten">The number of bytes successfully written gets written here.</param>
        /// <returns>True, if all bytes were written suvvessfully.</returns>
        public bool TryWriteBytes(Span<byte> destination, out int bytesWritten)
        {
            var bytes = this.Bytes.Span;
            var minSize = Math.Min(destination.Length, bytes.Length);
            for (var i = 0; i < minSize; ++i) destination[i] = bytes[i];
            bytesWritten = minSize;
            return destination.Length >= bytes.Length;
        }

        /// <summary>
        /// Creates a <see cref="BigInt"/> from a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="width">The width of the resulting <see cref="BigInt"/> in bits.</param>
        /// <param name="bigInteger">The <see cref="BigInteger"/> to convert.</param>
        /// <returns>A new <see cref="BigInt"/> with equivalent value to <paramref name="bigInteger"/>.</returns>
        public static BigInt FromBigInteger(int width, BigInteger bigInteger)
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

            return new(width, builder);
        }

        /// <summary>
        /// Converts this <see cref="BigInt"/> to a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="signed">True, if the bytes should be interpreted as signed.</param>
        /// <returns>A <see cref="BigInteger"/> equivalent to this number.</returns>
        public BigInteger ToBigInteger(bool signed)
        {
            // On byte-border or unsigned, simple conversion
            if (this.Width % 8 == 0 || !signed || !this.SignBit) return new(this.Bytes.Span, !signed);

            // It's signed but the sign bit is not aligned to a byte border
            var builder = this.builder.Clone();
            var fillMaskIndex = (8 - (this.Width % 8)) % 8;
            var fillMask = TopFillMasks[fillMaskIndex];
            builder.Bytes.Span[^1] |= fillMask;
            return new(builder.Bytes.Span, false);
        }

        /// <summary>
        /// Returns the smallest <see cref="BigInt"/> possible with the given width.
        /// </summary>
        /// <param name="signed">True, if should be the signed minimum value.</param>
        /// <param name="width">The width of the requested <see cref="BigInt"/>.</param>
        /// <returns>The smallest <see cref="BigInt"/> possible with the given width and signedness.</returns>
        public static BigInt MinValue(bool signed, int width)
        {
            var builder = BigIntBuilder.WithBitCapacity(width);
            if (signed) builder[width - 1] = true;
            return new(width, builder);
        }

        /// <summary>
        /// Returns the largest <see cref="BigInt"/> possible with the given width.
        /// </summary>
        /// <param name="signed">True, if should be the signed maximum value.</param>
        /// <param name="width">The width of the requested <see cref="BigInt"/>.</param>
        /// <returns>The largest <see cref="BigInt"/> possible with the given width and signedness.</returns>
        public static BigInt MaxValue(bool signed, int width)
        {
            var builder = BigIntBuilder.WithBitCapacity(width);
            builder.SetAllBitsOne();
            builder.MaskToWidth(width);
            if (signed) builder[width - 1] = false;
            return new(width, builder);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BigInt other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(BigInt other) => this.UnsignedCompareTo(other) == 0;

        /// <summary>
        /// Compates this <see cref="BigInt"/> with another using unsigned comparison.
        /// </summary>
        /// <param name="other">The other <see cref="BigInt"/> to compare with.</param>
        /// <returns>Negative number, if this is smaller than <paramref name="other"/>,
        /// 0 if they are equal, positive number otherwise.</returns>
        public int UnsignedCompareTo(BigInt other) => BigIntBuilder.Compare(this.builder.Bytes.Span, other.builder.Bytes.Span);

        /// <summary>
        /// Compates this <see cref="BigInt"/> with another using signed comparison.
        /// </summary>
        /// <param name="other">The other <see cref="BigInt"/> to compare with.</param>
        /// <returns>Negative number, if this is smaller than <paramref name="other"/>,
        /// 0 if they are equal, positive number otherwise.</returns>
        public int SignedCompareTo(BigInt other)
        {
            var cmp = this.UnsignedCompareTo(other);
            if (this.SignBit != other.SignBit) cmp = -cmp;
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
        /// Bitwise negates the given <see cref="BigInt"/>.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>A bitwise negation of <paramref name="bigInt"/>.</returns>
        public static BigInt BitwiseNegate(BigInt bigInt)
        {
            var builder = bigInt.builder.Clone();
            builder.BitwiseNegate();
            builder.MaskToWidth(bigInt.Width);
            return new(bigInt.Width, builder);
        }

        /// <summary>
        /// Negates a <see cref="BigInt"/> using twos-complement.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>The negated <paramref name="bigInt"/>.</returns>
        public static BigInt Negate(BigInt bigInt)
        {
            var builder = bigInt.builder.Clone();
            builder.TwosComplement();
            builder.MaskToWidth(bigInt.Width);
            return new(bigInt.Width, builder);
        }

        /// <summary>
        /// Adds two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="lhs">The first <see cref="BigInt"/> to add.</param>
        /// <param name="rhs">The second <see cref="BigInt"/> to add.</param>
        /// <returns>A new <see cref="BigInt"/>, that is the sum of <paramref name="lhs"/>
        /// and <paramref name="rhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Add(BigInt lhs, BigInt rhs) => Add(lhs, rhs, out _);

        /// <summary>
        /// Adds two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="lhs">The first <see cref="BigInt"/> to add.</param>
        /// <param name="rhs">The second <see cref="BigInt"/> to add.</param>
        /// <param name="overflow">True gets written here, if overflow happened.</param>
        /// <returns>A new <see cref="BigInt"/>, that is the sum of <paramref name="lhs"/>
        /// and <paramref name="rhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Add(BigInt lhs, BigInt rhs, out bool overflow)
        {
            var builder = lhs.builder.Clone();
            builder.Add(rhs.Bytes.Span, out overflow);
            overflow = builder.MaskToWidth(lhs.Width) || overflow;
            return new(lhs.Width, builder);
        }

        /// <summary>
        /// Subtracts two <see cref="BigInt"/>s from eachother.
        /// </summary>
        /// <param name="lhs">The <see cref="BigInt"/> to subtract from.</param>
        /// <param name="rhs">The <see cref="BigInt"/> to subtract.</param>
        /// <returns>A new <see cref="BigInt"/>, that is <paramref name="rhs"/> subtracted from
        /// <paramref name="lhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Subtract(BigInt lhs, BigInt rhs) => Subtract(lhs, rhs, out _);

        /// <summary>
        /// Subtracts two <see cref="BigInt"/>s from eachother.
        /// </summary>
        /// <param name="lhs">The <see cref="BigInt"/> to subtract from.</param>
        /// <param name="rhs">The <see cref="BigInt"/> to subtract.</param>
        /// <param name="underflow">True gets written here, if underflow happened.</param>
        /// <returns>A new <see cref="BigInt"/>, that is <paramref name="rhs"/> subtracted from
        /// <paramref name="lhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Subtract(BigInt lhs, BigInt rhs, out bool underflow)
        {
            var builder = lhs.builder.Clone();
            builder.Subtract(rhs.Bytes.Span, out underflow);
            underflow = builder.MaskToWidth(lhs.Width) || underflow;
            return new(lhs.Width, builder);
        }

        /// <summary>
        /// Multiplies two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="lhs">The first <see cref="BigInt"/> to multiply.</param>
        /// <param name="rhs">The second <see cref="BigInt"/> to multiply.</param>
        /// <returns>A new <see cref="BigInt"/>, that is the product of <paramref name="lhs"/>
        /// and <paramref name="rhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Multiply(BigInt lhs, BigInt rhs) => Multiply(lhs, rhs, out _);

        /// <summary>
        /// Multiplies two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="lhs">The first <see cref="BigInt"/> to multiply.</param>
        /// <param name="rhs">The second <see cref="BigInt"/> to multiply.</param>
        /// <param name="overflow">True gets written here, if overflow happened.</param>
        /// <returns>A new <see cref="BigInt"/>, that is the product of <paramref name="lhs"/>
        /// and <paramref name="rhs"/>, and its width will be the width of <paramref name="lhs"/>.</returns>
        public static BigInt Multiply(BigInt lhs, BigInt rhs, out bool overflow)
        {
            var builder = lhs.builder.Clone();
            builder.Multiply(rhs.Bytes.Span, out overflow);
            overflow = builder.MaskToWidth(lhs.Width) || overflow;
            return new(lhs.Width, builder);
        }

        /// <summary>
        /// Divides a <see cref="BigInt"/> with another.
        /// </summary>
        /// <param name="lhs">The <see cref="BigInt"/> to divide.</param>
        /// <param name="rhs">The <see cref="BigInt"/> to divide by.</param>
        /// <param name="remainder">The remainder gets written here. Its width will be
        /// the width of <paramref name="lhs"/>.</param>
        /// <returns>A new <see cref="BigInt"/> that is the result of dividing
        /// <paramref name="lhs"/> by <paramref name="rhs"/> and has the width of <paramref name="lhs"/>.</returns>
        public static BigInt Divide(BigInt lhs, BigInt rhs, out BigInt remainder)
        {
            var builder = lhs.builder.Clone();
            builder.Divide(rhs.Bytes.Span, out var remBuilder);
            remainder = new(lhs.Width, remBuilder);
            return new(lhs.Width, builder);
        }

        /// <summary>
        /// Divides a <see cref="BigInt"/> with another.
        /// </summary>
        /// <param name="lhs">The <see cref="BigInt"/> to divide.</param>
        /// <param name="rhs">The <see cref="BigInt"/> to divide by.</param>
        /// <returns>A new <see cref="BigInt"/> that is the result of dividing
        /// <paramref name="lhs"/> by <paramref name="rhs"/> and has the width of <paramref name="lhs"/>.</returns>
        public static BigInt Divide(BigInt lhs, BigInt rhs) => Divide(lhs, rhs, out _);

        /// <summary>
        /// Calculates the remainder when dividing a <see cref="BigInt"/> with another.
        /// </summary>
        /// <param name="lhs">The <see cref="BigInt"/> to divide.</param>
        /// <param name="rhs">The <see cref="BigInt"/> to divide by.</param>
        /// <returns>A new <see cref="BigInt"/> that is the remainder when dividing
        /// <paramref name="lhs"/> by <paramref name="rhs"/>. The remainder has the width
        /// of <paramref name="lhs"/>.</returns>
        public static BigInt Modulo(BigInt lhs, BigInt rhs)
        {
            _ = Divide(lhs, rhs, out var rem);
            return rem;
        }

        /// <summary>
        /// Ands together the bits of two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to and.</param>
        /// <param name="right">The second <see cref="BigInt"/> to and.</param>
        /// <returns>The result of anding together the bits of <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt BitwiseAnd(BigInt left, BigInt right)
        {
            var builder = left.builder.Clone();
            builder.BitwiseAnd(right.Bytes.Span);
            return new(left.Width, builder);
        }

        /// <summary>
        /// Ors together the bits of two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to or.</param>
        /// <param name="right">The second <see cref="BigInt"/> to or.</param>
        /// <returns>The result of oring together the bits of <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt BitwiseOr(BigInt left, BigInt right)
        {
            var builder = left.builder.Clone();
            builder.BitwiseOr(right.Bytes.Span);
            return new(left.Width, builder);
        }

        /// <summary>
        /// Xors together the bits of two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to xor.</param>
        /// <param name="right">The second <see cref="BigInt"/> to xor.</param>
        /// <returns>The result of xoring together the bits of <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt BitwiseXor(BigInt left, BigInt right)
        {
            var builder = left.builder.Clone();
            builder.BitwiseXor(right.Bytes.Span);
            builder.MaskToWidth(left.Width);
            return new(left.Width, builder);
        }

        /// <summary>
        /// Bitwise shifts a given <see cref="BigInt"/> left by a given amount.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to shift.</param>
        /// <param name="amount">The amount to shift by.</param>
        /// <returns>The result if shifting <paramref name="bigInt"/> by <paramref name="amount"/>
        /// to the left.</returns>
        public static BigInt ShiftLeft(BigInt bigInt, int amount)
        {
            var builder = bigInt.builder.Clone();
            builder.ShiftLeft(amount);
            return new(bigInt.Width, builder);
        }

        /// <summary>
        /// Bitwise shifts a given <see cref="BigInt"/> right by a given amount.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to shift.</param>
        /// <param name="amount">The amount to shift by.</param>
        /// <returns>The result if shifting <paramref name="bigInt"/> by <paramref name="amount"/>
        /// to the right.</returns>
        public static BigInt ShiftRight(BigInt bigInt, int amount)
        {
            var builder = bigInt.builder.Clone();
            builder.ShiftRight(amount);
            return new(bigInt.Width, builder);
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
        /// Bitwise negates the given <see cref="BigInt"/>.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>A bitwise negation of <paramref name="bigInt"/>.</returns>
        public static BigInt operator ~(BigInt bigInt) => BitwiseNegate(bigInt);

        /// <summary>
        /// Negates a <see cref="BigInt"/> using twos-complement.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInt"/> to negate.</param>
        /// <returns>The negated <paramref name="bigInt"/>.</returns>
        public static BigInt operator -(BigInt bigInt) => Negate(bigInt);

        /// <summary>
        /// Adds two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to add.</param>
        /// <param name="right">The second <see cref="BigInt"/> to add.</param>
        /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/>
        /// with the width of <paramref name="left"/>.</returns>
        public static BigInt operator +(BigInt left, BigInt right) => Add(left, right);

        /// <summary>
        /// Subtracts two <see cref="BigInt"/>s from eachother.
        /// </summary>
        /// <param name="left">The <see cref="BigInt"/> to subtract from.</param>
        /// <param name="right">The <see cref="BigInt"/> to subtract.</param>
        /// <returns>The result of <paramref name="right"/> subtracted from <paramref name="left"/>.
        /// The width of the result will be the width of <paramref name="left"/>.</returns>
        public static BigInt operator -(BigInt left, BigInt right) => Subtract(left, right);

        /// <summary>
        /// Multiplies two <see cref="BigInt"/>s together.
        /// </summary>
        /// <param name="left">The first <see cref="BigInt"/> to multiply.</param>
        /// <param name="right">The second <see cref="BigInt"/> to multiply.</param>
        /// <returns>The product of <paramref name="left"/> and <paramref name="right"/>
        /// with the width of <paramref name="left"/>.</returns>
        public static BigInt operator *(BigInt left, BigInt right) => Multiply(left, right);

        /// <summary>
        /// Divides a <see cref="BigInt"/> by another.
        /// </summary>
        /// <param name="left">The <see cref="BigInt"/> to divide.</param>
        /// <param name="right">The <see cref="BigInt"/> to divide by.</param>
        /// <returns>The result of dividing <paramref name="left"/> by <paramref name="right"/>
        /// with the width of <paramref name="left"/>.</returns>
        public static BigInt operator /(BigInt left, BigInt right) => Divide(left, right);

        /// <summary>
        /// Calculates the remainder when dividing a <see cref="BigInt"/> by another.
        /// </summary>
        /// <param name="left">The <see cref="BigInt"/> to divide.</param>
        /// <param name="right">The <see cref="BigInt"/> to divide by.</param>
        /// <returns>The remainder when dividing <paramref name="left"/> by <paramref name="right"/>
        /// with the width of <paramref name="left"/>.</returns>
        public static BigInt operator %(BigInt left, BigInt right) => Modulo(left, right);

        /// <summary>
        /// Bitwise ands together two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The fist <see cref="BigInt"/> to bitwise-and.</param>
        /// <param name="right">The second <see cref="BigInt"/> to bitwise-and.</param>
        /// <returns>The result of bitwise and-ing together <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt operator &(BigInt left, BigInt right) => BitwiseAnd(left, right);

        /// <summary>
        /// Bitwise ors together two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The fist <see cref="BigInt"/> to bitwise-or.</param>
        /// <param name="right">The second <see cref="BigInt"/> to bitwise-or.</param>
        /// <returns>The result of bitwise or-ing together <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt operator |(BigInt left, BigInt right) => BitwiseOr(left, right);

        /// <summary>
        /// Bitwise xors together two <see cref="BigInt"/>s.
        /// </summary>
        /// <param name="left">The fist <see cref="BigInt"/> to bitwise-xor.</param>
        /// <param name="right">The second <see cref="BigInt"/> to bitwise-xor.</param>
        /// <returns>The result of bitwise xor-ing together <paramref name="left"/> and
        /// <paramref name="right"/> with the width of <paramref name="left"/>.</returns>
        public static BigInt operator ^(BigInt left, BigInt right) => BitwiseXor(left, right);

        /// <summary>
        /// Bitwise-shifts a <see cref="BigInt"/> left by a given amount.
        /// </summary>
        /// <param name="left">The <see cref="BigInt"/> to shift.</param>
        /// <param name="right">The amount to shift by.</param>
        /// <returns>The result of shifting <paramref name="left"/> left by <paramref name="right"/>.</returns>
        public static BigInt operator <<(BigInt left, int right) => ShiftLeft(left, right);

        /// <summary>
        /// Bitwise-shifts a <see cref="BigInt"/> right by a given amount.
        /// </summary>
        /// <param name="left">The <see cref="BigInt"/> to shift.</param>
        /// <param name="right">The amount to shift by.</param>
        /// <returns>The result of shifting <paramref name="left"/> right by <paramref name="right"/>.</returns>
        public static BigInt operator >>(BigInt left, int right) => ShiftRight(left, right);
    }
}
