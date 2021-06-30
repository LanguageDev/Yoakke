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
    // TODO: This does not differentiate AL from AH
    // Maybe we should stick to reference semantics

    /// <summary>
    /// Represents a single x86 register.
    /// </summary>
    public class Register : IEquatable<Register>
    {
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
        /// <param name="width">The width of the register in bytes.</param>
        /// <param name="contained">The contained registers of this one.</param>
        public Register(int width, params Register[] contained)
        {
            this.Width = width;
            this.Contained = contained;
        }

        public override bool Equals(object? obj) => this.Equals(obj as Register);

        public bool Equals(Register? other) =>
               other is not null
            && this.Width == other.Width
            && this.Contained.SequenceEqual(other.Contained);

        public override int GetHashCode()
        {
            var hash = default(HashCode);
            hash.Add(this.Width);
            foreach (var reg in this.Contained) hash.Add(reg);
            return hash.ToHashCode();
        }
    }
}
