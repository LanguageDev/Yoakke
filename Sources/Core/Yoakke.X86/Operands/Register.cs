// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Operands
{
    /// <summary>
    /// Represents a single x86 register.
    /// </summary>
    public readonly struct Register : IOperand
    {
        public bool IsMemory => false;

        /// <summary>
        /// The name of this <see cref="Register"/>.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The width of this <see cref="Register"/> in bytes.
        /// </summary>
        public readonly DataWidth Size;

        /// <summary>
        /// The contained <see cref="Register"/> or <see cref="Register"/>s of this one.
        /// </summary>
        public readonly IReadOnlyList<Register> Contained;

        /// <summary>
        /// Initializes a new instance of the <see cref="Register"/> struct.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <param name="size">The width of the register in bytes.</param>
        /// <param name="contained">The contained registers of this one.</param>
        public Register(string name, DataWidth size, params Register[] contained)
        {
            this.Name = name;
            this.Size = size;
            this.Contained = contained;
        }
    }
}
