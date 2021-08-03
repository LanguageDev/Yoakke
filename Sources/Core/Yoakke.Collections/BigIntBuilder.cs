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
        /// True, if all bytes are 0.
        /// </summary>
        public bool IsZero
        {
            get
            {
                for (var i = 0; i < this.Bytes.Length; ++i)
                {
                    if (this.Bytes.Span[i] != 0) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// True, if even.
        /// </summary>
        public bool IsEven => this.Bytes.Span[0] % 2 == 0;

        /// <summary>
        /// True, if odd.
        /// </summary>
        public bool IsOdd => this.Bytes.Span[0] % 2 == 1;

        /// <summary>
        /// Provides bitwise access.
        /// </summary>
        /// <param name="index">The index of the accessed bit.</param>
        /// <returns>True, if bit at <paramref name="index"/> is set to 1.</returns>
        public bool this[int index]
        {
            get => (this.Bytes.Span[index / 8] >> (index % 8)) % 2 != 0;
            set
            {
                if (value) this.Bytes.Span[index / 8] |= (byte)(1 << (index % 8));
                else this.Bytes.Span[index / 8] &= (byte)~(1 << (index % 8));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigIntBuilder"/> class.
        /// </summary>
        /// <param name="bytes">The bytes to modify in-place.</param>
        public BigIntBuilder(Memory<byte> bytes)
        {
            this.Bytes = bytes;
        }

        /// <summary>
        /// Sets all bytes to the same value.
        /// </summary>
        /// <param name="value">The value to set all bytes to.</param>
        public void Set(byte value)
        {
            for (var i = 0; i < this.Bytes.Length; ++i) this.Bytes.Span[i] = value;
        }

        /// <summary>
        /// Sets all bits to 0.
        /// </summary>
        public void SetAllBitsZero() => this.Set(0);

        /// <summary>
        /// Sets all bits to 1.
        /// </summary>
        public void SetAllBitsOne() => this.Set(0b11111111);

        /// <summary>
        /// Bitwise negates the number.
        /// </summary>
        public void BitwiseNegate()
        {
            for (var i = 0; i < this.Bytes.Length; ++i) this.Bytes.Span[i] = (byte)~this.Bytes.Span[i];
        }

        /// <summary>
        /// Performs 2s complement.
        /// </summary>
        public void TwosComplement()
        {
            this.BitwiseNegate();
            this.Add(One, out _);
        }

        /// <summary>
        /// Adds bytes to this.
        /// </summary>
        /// <param name="other">The bytes to add.</param>
        /// <param name="overflow">True gets written here, if overflow happened.</param>
        public void Add(ReadOnlySpan<byte> other, out bool overflow)
        {
            if (other.Length > this.Bytes.Length)
            {
                throw new ArgumentException("There can't be more bytes added than the held number of bytes", nameof(other));
            }

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
        /// Subtracts bytes from this.
        /// </summary>
        /// <param name="other">The bytes to add. Will be in-place modified.</param>
        /// <param name="underflow">True gets written here, if underflow happened.</param>
        public void Subtract(ReadOnlySpan<byte> other, out bool underflow) =>
            this.Subtract(new BigIntBuilder(other.ToArray()), out underflow);

        /// <summary>
        /// Subtracts bytes from this.
        /// </summary>
        /// <param name="other">The bytes to add. Will be in-place modified.</param>
        /// <param name="underflow">True gets written here, if underflow happened.</param>
        public void Subtract(BigIntBuilder other, out bool underflow)
        {
            other.TwosComplement();
            this.Add(other.Bytes.Span, out underflow);
        }

        /// <summary>
        /// Multiplies another number with this.
        /// </summary>
        /// <param name="other">The bytes to multiply with. Will be in-place modified.</param>
        /// <param name="overflow">True gets written here, if underflow happened.</param>
        public void Multiply(ReadOnlySpan<byte> other, out bool overflow) =>
            this.Multiply(new BigIntBuilder(other.ToArray()), out overflow);

        /// <summary>
        /// Multiplies another number with this.
        /// </summary>
        /// <param name="other">The bytes to multiply with. Will be in-place modified.</param>
        /// <param name="overflow">True gets written here, if underflow happened.</param>
        public void Multiply(BigIntBuilder other, out bool overflow)
        {
            var left = new BigIntBuilder(this.Bytes.ToArray());
            this.SetAllBitsZero();
            overflow = false;

            // Russian peasant's method
            while (!other.IsZero)
            {
                if (other.IsOdd)
                {
                    this.Add(this.Bytes.Span, out var o);
                    overflow = overflow || o;
                }

                left.ShiftLeft(1);
                other.ShiftRight(1);
            }
        }

        // TODO: Divide

        /// <summary>
        /// Performs bitwise-and.
        /// </summary>
        /// <param name="other">The bytes to bitwise-and with.</param>
        public void BitwiseAnd(ReadOnlySpan<byte> other)
        {
            var minLength = Math.Min(this.Bytes.Length, other.Length);
            for (var i = 0; i < minLength; ++i) this.Bytes.Span[i] &= other[i];
        }

        /// <summary>
        /// Performs bitwise-or.
        /// </summary>
        /// <param name="other">The bytes to bitwise-or with.</param>
        public void BitwiseOr(ReadOnlySpan<byte> other)
        {
            var minLength = Math.Min(this.Bytes.Length, other.Length);
            for (var i = 0; i < minLength; ++i) this.Bytes.Span[i] |= other[i];
        }

        /// <summary>
        /// Performs bitwise-xor.
        /// </summary>
        /// <param name="other">The bytes to bitwise-xor with.</param>
        public void BitwiseXor(ReadOnlySpan<byte> other)
        {
            var minLength = Math.Min(this.Bytes.Length, other.Length);
            for (var i = 0; i < minLength; ++i) this.Bytes.Span[i] ^= other[i];
        }

        /// <summary>
        /// Performs bitwise left-shift.
        /// </summary>
        /// <param name="amount">The amount to shift.</param>
        public void ShiftLeft(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            var byteShift = amount / 8;
            var bitShift = amount % 8;
            var shiftBack = 8 - bitShift;
            var topMask = (byte)(0xff << shiftBack);

            var carry = 0;
            for (var i = this.Bytes.Length - 1 - byteShift; i >= 0; --i)
            {
                var toIndex = i + byteShift;
                var fromByte = this.Bytes.Span[i];
                var nextCarry = (fromByte & topMask) >> shiftBack;
                this.Bytes.Span[toIndex] = (byte)((fromByte << bitShift) | carry);
                carry = nextCarry;
            }
        }

        /// <summary>
        /// Performs bitwise right-shift.
        /// </summary>
        /// <param name="amount">The amount to shift.</param>
        public void ShiftRight(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            var byteShift = amount / 8;
            var bitShift = amount % 8;
            var shiftFront = 8 - bitShift;
            var bottomMask = (byte)(0xff >> shiftFront);

            var carry = 0;
            for (var i = byteShift; i < this.Bytes.Length; ++i)
            {
                var toIndex = i - byteShift;
                var fromByte = this.Bytes.Span[i];
                var nextCarry = (fromByte & bottomMask) << shiftFront;
                this.Bytes.Span[toIndex] = (byte)((fromByte >> bitShift) | carry);
                carry = nextCarry;
            }
        }

        /// <summary>
        /// Compares two byte sequences as unsigned numbers.
        /// </summary>
        /// <param name="left">The first number to compare.</param>
        /// <param name="right">The second number to compare.</param>
        /// <returns>Less than 0 if <paramref name="left"/> is smaller than <paramref name="right"/>,
        /// 0 if they are equal, greater than 0 otherwise.</returns>
        public static int Compare(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            if (left.Length != right.Length) throw new ArgumentException("The two byte sequences must be of same length");

            for (var i = left.Length - 1; i >= 0; --i)
            {
                var diff = left[i] - right[i];
                if (diff != 0) return diff;
            }
            return 0;
        }
    }
}
