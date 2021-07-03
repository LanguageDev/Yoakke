// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Instructions
{
    /// <summary>
    /// A single x86 instruction.
    /// </summary>
    public interface IInstruction : IAssemblyElement
    {
        /// <summary>
        /// The <see cref="X86.Opcode"/> the <see cref="IInstruction"/> executes.
        /// </summary>
        public Opcode Opcode { get; }

        /// <summary>
        /// The <see cref="IOperand"/>s this <see cref="IInstruction"/> needs.
        /// </summary>
        public IEnumerable<IOperand> Operands { get; }
    }
}
