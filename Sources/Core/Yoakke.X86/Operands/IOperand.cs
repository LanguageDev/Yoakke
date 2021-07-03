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
        /// The <see cref="DataWidth"/> - or size - of this <see cref="IOperand"/>, if can be determined.
        /// </summary>
        public DataWidth? Size { get; }
    }
}
