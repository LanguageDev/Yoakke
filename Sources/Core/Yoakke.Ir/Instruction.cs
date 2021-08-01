// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// BBase for all instructions.
    /// </summary>
    public abstract record Instruction
    {
        /// <summary>
        /// True, if this is some kind of branching instruction.
        /// </summary>
        public abstract bool IsBranch { get; }

        /// <summary>
        /// Any <see cref="Instruction"/> that produces a <see cref="Value"/>.
        /// </summary>
        public abstract record ValueProducer : Instruction
        {
            /// <inheritdoc/>
            public override bool IsBranch => false;

            /// <summary>
            /// The result <see cref="Type"/> of this instruction.
            /// </summary>
            public abstract Type ResultType { get; }
        }

        /// <summary>
        /// Returns from the current procedure.
        /// </summary>
        public record Ret(Value? Value) : Instruction
        {
            /// <inheritdoc/>
            public override bool IsBranch => true;
        }

        /// <summary>
        /// Integer addition.
        /// </summary>
        public record IntAdd(Value Left, Value Right) : ValueProducer
        {
            /// <inheritdoc/>
            public override Type ResultType => this.Left.Type;
        }
    }
}
