// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.X86.Operands
{
    /// <summary>
    /// Represents an x86 segment register.
    /// </summary>
    public readonly struct Segment : IOperand
    {
        /// <inheritdoc/>
        public bool IsMemory => false;

        /// <inheritdoc/>
        public DataWidth? GetSize() => null;

        /// <inheritdoc/>
        public DataWidth GetSize(AssemblyContext context) => context.AddressSize;

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
