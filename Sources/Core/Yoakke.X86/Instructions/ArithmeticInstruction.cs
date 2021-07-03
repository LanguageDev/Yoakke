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
    /// An <see cref="IInstruction"/> that is like the ADD instruction in x86.
    /// </summary>
    public abstract class ArithmeticInstruction : IInstruction
    {
        public Opcode Opcode { get; }

        public IEnumerable<IOperand> Operands
        {
            get
            {
                yield return this.Destination;
                yield return this.Source;
            }
        }

        /// <summary>
        /// The destination <see cref="IOperand"/>.
        /// </summary>
        public IOperand Destination { get; }

        /// <summary>
        /// The source <see cref="IOperand"/>.
        /// </summary>
        public IOperand Source { get; }

        protected ArithmeticInstruction(Opcode opcode, IOperand dest, IOperand src)
        {
            if (dest.IsMemory && src.IsMemory) throw new ArgumentException("arithmetic operations require at least one non-memory argument");
            if (dest is Constant) throw new ArgumentException("destination cannot be a constant", nameof(dest));

            this.Opcode = opcode;
            this.Destination = dest;
            this.Source = src;
        }
    }
}
