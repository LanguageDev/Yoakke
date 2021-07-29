// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Operands
{
    /// <summary>
    /// A far-jump address with a segment.
    /// </summary>
    public readonly struct FarAddress : IOperand
    {
        /// <inheritdoc/>
        public bool IsMemory => true;

        /// <inheritdoc/>
        public DataWidth? GetSize() => null;

        /// <inheritdoc/>
        public DataWidth GetSize(AssemblyContext context) => context.AddressSize;

        /// <summary>
        /// The segment selector.
        /// </summary>
        public readonly int Segment;

        /// <summary>
        /// The address.
        /// </summary>
        public readonly int Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="FarAddress"/> struct.
        /// </summary>
        /// <param name="segment">The segment selector.</param>
        /// <param name="address">The address.</param>
        public FarAddress(int segment, int address)
        {
            this.Segment = segment;
            this.Address = address;
        }
    }
}
