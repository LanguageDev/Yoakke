// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Operands
{
    /// <summary>
    /// A constant operand.
    /// </summary>
    public readonly struct Constant : IOperand
    {
        /// <inheritdoc/>
        public bool IsMemory => false;

        /// <inheritdoc/>
        public DataWidth? GetSize() => this.Size;

        /// <inheritdoc/>
        public DataWidth GetSize(AssemblyContext context) => this.Size;

        /// <summary>
        /// The width of the constant.
        /// </summary>
        public readonly DataWidth Size;

        /// <summary>
        /// The value of this <see cref="Constant"/>.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="size">The <see cref="DataWidth"/> of this constant.</param>
        /// <param name="value">The value object.</param>
        internal Constant(DataWidth size, object value)
        {
            this.Size = size;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public Constant(int value)
            : this(DataWidth.Dword, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="value">The long value.</param>
        public Constant(long value)
            : this(DataWidth.Qword, value)
        {
        }
    }
}
