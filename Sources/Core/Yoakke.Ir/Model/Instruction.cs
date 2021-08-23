// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// BBase for all instructions.
    /// </summary>
    public abstract partial record Instruction
    {
        /// <summary>
        /// The operands of this <see cref="Instruction"/>.
        /// </summary>
        public IReadOnlyList<IInstructionArg> Operands { get; }

        /// <summary>
        /// True, if this is some kind of branching instruction.
        /// </summary>
        public virtual bool IsBranch => false;

        /// <summary>
        /// True, if this is a side-effect-free instruction.
        /// </summary>
        public virtual bool IsPure => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="operands">The operannds of this <see cref="Instruction"/>.</param>
        public Instruction(params IInstructionArg[] operands)
        {
            this.Operands = operands;
        }

        /// <summary>
        /// Any <see cref="Instruction"/> that produces a <see cref="Value"/>.
        /// </summary>
        public abstract record ValueProducer : Instruction
        {
            /// <summary>
            /// The result <see cref="Type"/> of this instruction.
            /// </summary>
            public abstract Type ResultType { get; }

            /// <inheritdoc/>
            public override bool IsPure => true;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueProducer"/> class.
            /// </summary>
            /// <param name="operands">The operands of this <see cref="Instruction"/>.</param>
            public ValueProducer(params IInstructionArg[] operands)
                : base(operands)
            {
            }
        }

        /// <summary>
        /// A binary operation.
        /// </summary>
        public abstract record Binary(Value Left, Value Right) : ValueProducer(Left, Right)
        {
            /// <inheritdoc/>
            public override Type ResultType => this.Left.Type;
        }

        /// <summary>
        /// Calls a procedure.
        /// </summary>
        public record Call(Value Procedure, IReadOnlyValueList<Value> Arguments)
            : ValueProducer(Arguments.Prepend(Procedure).ToArray())
        {
            /// <inheritdoc/>
            public override bool IsPure => false;

            /// <inheritdoc/>
            public override Type ResultType => ((Type.Proc)this.Procedure.Type).Return;
        }

        /// <summary>
        /// Returns from the current procedure.
        /// </summary>
        public record Ret(Value? Value)
            : Instruction(Value is null ? Array.Empty<Value>() : new[] { Value })
        {
            /// <inheritdoc/>
            public override bool IsBranch => true;
        }

        /// <summary>
        /// An unconditional jump.
        /// </summary>
        public record Jump(Value Target) : Instruction(Target)
        {
            /// <inheritdoc/>
            public override bool IsBranch => true;

            /// <inheritdoc/>
            public override bool IsPure => false;
        }

        /// <summary>
        /// A conditional jump.
        /// </summary>
        public record JumpIf(Value Condition, Value Then, Value Else)
            : Instruction(Condition, Then, Else)
        {
            /// <inheritdoc/>
            public override bool IsBranch => true;
        }

        /// <summary>
        /// Addition.
        /// </summary>
        public record Add(Value Left, Value Right) : Binary(Left, Right);

        /// <summary>
        /// Subtraction.
        /// </summary>
        public record Sub(Value Left, Value Right) : Binary(Left, Right);

        /// <summary>
        /// Comparison (less, equal, greater).
        /// </summary>
        public record Cmp(Comparison Comparison, Value Left, Value Right)
            : Binary(Left, Right)
        {
            /// <inheritdoc/>
            public override Type ResultType => new Type.Int(false, 1);
        }
    }
}
