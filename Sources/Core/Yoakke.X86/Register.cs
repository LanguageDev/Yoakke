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
    /// Represents a single x86 register.
    /// </summary>
    public class Register
    {
        /// <summary>
        /// The name of this <see cref="Register"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The width of this <see cref="Register"/> in bytes.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The contained <see cref="Register"/> or <see cref="Register"/>s of this one.
        /// </summary>
        public IReadOnlyList<Register> Contained { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Register"/> class.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <param name="width">The width of the register in bytes.</param>
        /// <param name="contained">The contained registers of this one.</param>
        public Register(string name, int width, params Register[] contained)
        {
            this.Name = name;
            this.Width = width;
            this.Contained = contained;
        }
    }
}
