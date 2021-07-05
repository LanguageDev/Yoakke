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
    /// Operands for x86 instructions.
    /// </summary>
    public interface IOperand
    {
        /// <summary>
        /// True, if this operand involves memory read/write.
        /// </summary>
        public bool IsMemory { get; }

        /// <summary>
        /// Retrieves the <see cref="DataWidth"/> - or size - of the resulting valud of this <see cref="IOperand"/>
        /// without any context.
        /// </summary>
        /// <returns>The <see cref="DataWidth"/> that corresponds to the size of this operand, or null
        /// if it cannot be determined without context.</returns>
        public DataWidth? GetSize();

        /// <summary>
        /// Retrieves the <see cref="DataWidth"/> - or size - of the resulting valud of this <see cref="IOperand"/>.
        /// </summary>
        /// <param name="context">The <see cref="AssemblyContext"/> that contains contextual information.</param>
        /// <returns>The <see cref="DataWidth"/> that corresponds to the size of this operand.</returns>
        public DataWidth GetSize(AssemblyContext context);
    }
}
