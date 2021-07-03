// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// A single x86 instruction.
    /// </summary>
    public readonly struct Instruction
    {
        /// <summary>
        /// The <see cref="X86.Opcode"/> the <see cref="Instruction"/> executes.
        /// </summary>
        public readonly Opcode Opcode;

        /// <summary>
        /// The <see cref="IOperand"/>s this <see cref="Instruction"/> needs.
        /// </summary>
        public readonly IReadOnlyList<IOperand> Operands;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> struct.
        /// </summary>
        /// <param name="opcode">The <see cref="X86.Opcode"/> the instruction executes.</param>
        /// <param name="operands">The <see cref="IOperand"/>s the instruction needs.</param>
        public Instruction(Opcode opcode, params IOperand[] operands)
        {
            this.Opcode = opcode;
            this.Operands = operands;
        }
    }
}
