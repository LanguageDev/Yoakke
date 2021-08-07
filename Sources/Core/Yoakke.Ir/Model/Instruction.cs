// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// BBase for all instructions.
    /// </summary>
    public abstract record Instruction
    {
        /// <summary>
        /// The operands of this <see cref="Instruction"/>.
        /// </summary>
        public abstract IEnumerable<IInstructionArg> Operands { get; }

        /// <summary>
        /// True, if this is some kind of branching instruction.
        /// </summary>
        public virtual bool IsBranch => false;

        /// <summary>
        /// True, if this is a side-effect-free instruction.
        /// </summary>
        public virtual bool IsPure => true;

        /// <summary>
        /// Any <see cref="Instruction"/> that produces a <see cref="Value"/>.
        /// </summary>
        public abstract record ValueProducer : Instruction
        {
            /// <summary>
            /// The result <see cref="Type"/> of this instruction.
            /// </summary>
            public abstract Type ResultType { get; }
        }

        /// <summary>
        /// Calls a procedure.
        /// </summary>
        public record Call(Value Procedure, IReadOnlyValueList<Value> Arguments) : ValueProducer
        {
            /// <inheritdoc/>
            public override IEnumerable<IInstructionArg> Operands
            {
                get
                {
                    yield return this.Procedure;
                    foreach (var arg in this.Arguments) yield return arg;
                }
            }

            /// <inheritdoc/>
            public override bool IsPure => false;

            /// <inheritdoc/>
            public override Type ResultType => ((Type.Proc)this.Procedure.Type).Return;
        }

        /// <summary>
        /// Returns from the current procedure.
        /// </summary>
        public record Ret(Value? Value) : Instruction
        {
            /// <inheritdoc/>
            public override IEnumerable<IInstructionArg> Operands
            {
                get
                {
                    if (this.Value is not null) yield return this.Value;
                }
            }

            /// <inheritdoc/>
            public override bool IsBranch => true;

            /// <inheritdoc/>
            public override bool IsPure => false;
        }

        /// <summary>
        /// Integer addition.
        /// </summary>
        public record IntAdd(Value Left, Value Right) : ValueProducer
        {
            /// <inheritdoc/>
            public override IEnumerable<IInstructionArg> Operands
            {
                get
                {
                    yield return this.Left;
                    yield return this.Right;
                }
            }

            /// <inheritdoc/>
            public override Type ResultType => this.Left.Type;
        }
    }
}
