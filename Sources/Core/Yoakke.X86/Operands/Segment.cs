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
    /// Represents an x86 segment register.
    /// </summary>
    public readonly struct Segment : IOperand
    {
        public bool IsMemory => false;

        public DataWidth? Size => null;

        /// <summary>
        /// The name of this <see cref="Segment"/>.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> struct.
        /// </summary>
        /// <param name="name">The name of this segment.</param>
        public Segment(string name)
        {
            this.Name = name;
        }
    }
}
