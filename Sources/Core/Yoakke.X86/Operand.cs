// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86
{
    /// <summary>
    /// A scaled index for x86 addressing.
    /// </summary>
    public readonly struct ScaledIndex
    {
        /// <summary>
        /// The index that indexes the element.
        /// </summary>
        public readonly Register Index;

        /// <summary>
        /// The scale constant to scale the index by, that can be 1, 2, 4 or 8.
        /// </summary>
        public readonly int Scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaledIndex"/> struct.
        /// </summary>
        /// <param name="index">The index <see cref="Register"/>.</param>
        /// <param name="scale">The scale value.</param>
        public ScaledIndex(Register index, int scale)
        {
            if (scale != 1 && scale != 2 && scale != 4 && scale != 8)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "the scale must be 1, 2, 4 or 8");
            }

            this.Index = index;
            this.Scale = scale;
        }
    }

    public abstract class Operand
    {
        // TODO: Literal?
        // TODO: Symbol?

        /// <summary>
        /// An address specification.
        /// </summary>
        public class Address : Operand
        {
            /// <summary>
            /// The base address <see cref="Register"/>.
            /// </summary>
            public Register? Base { get; }

            /// <summary>
            /// An optional scaled offset from <see cref="Base"/>.
            /// </summary>
            public ScaledIndex? ScaledIndex { get; }

            /// <summary>
            /// A constant displacement.
            /// </summary>
            public int Displacement { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Address"/> class.
            /// </summary>
            /// <param name="base">The base address <see cref="Register"/>.</param>
            /// <param name="scaledIndex">An optional scaled offset.</param>
            /// <param name="displacement">A constant displacement.</param>
            public Address(Register? @base = null, ScaledIndex? scaledIndex = null, int displacement = 0)
            {
                this.Base = @base;
                this.ScaledIndex = scaledIndex;
                this.Displacement = displacement;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Address"/> class.
            /// </summary>
            /// <param name="displacement">A constant displacement.</param>
            public Address(int displacement)
                : this(null, null, displacement)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Address"/> class.
            /// </summary>
            /// <param name="base">The base address <see cref="Register"/>.</param>
            /// <param name="displacement">A constant displacement.</param>
            public Address(Register @base, int displacement)
                : this(@base, null, displacement)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Address"/> class.
            /// </summary>
            /// <param name="scaledIndex">An optional scaled offset.</param>
            /// <param name="displacement">A constant displacement.</param>
            public Address(ScaledIndex scaledIndex, int displacement = 0)
                : this(null, scaledIndex, displacement)
            {
            }
        }

        /// <summary>
        /// Indirect access through a memory address.
        /// </summary>
        public class Indirect : Operand
        {
            /// <summary>
            /// The width of the read.
            /// </summary>
            public int Width { get; }

            /// <summary>
            /// The address to read from.
            /// </summary>
            public new Operand Address { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Indirect"/> class.
            /// </summary>
            /// <param name="width">The width of the read.</param>
            /// <param name="address">The address to read from.</param>
            public Indirect(int width, Operand address)
            {
                this.Width = width;
                this.Address = address;
            }
        }
    }
}
