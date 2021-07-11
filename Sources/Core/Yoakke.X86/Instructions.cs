// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.X86;
using Yoakke.X86.Operands;
using Yoakke.X86.Validation;

namespace Yoakke.X86
{
    /// <summary>
    /// A wrapper class for all instructions.
    /// </summary>
    public static class Instructions
    {
        /* Base classes */

        /// <summary>
        /// Base-class for <see cref="IInstruction"/>s.
        /// </summary>
        public abstract record InstructionBase(IReadOnlyList<IOperand> Operands, string? Comment = null) : IInstruction;

        /// <summary>
        /// Base-class for zero-operand instructions.
        /// </summary>
        public abstract record ZeroOperandInstruction(string? Comment = null)
            : InstructionBase(Array.Empty<IOperand>(), Comment);

        /// <summary>
        /// Base-class for single-operand instructions.
        /// </summary>
        [Validator(typeof(OneOperandInstructionValidator))]
        public abstract record OneOperandInstruction(IOperand Operand, string? Comment = null)
            : InstructionBase(new[] { Operand }, Comment)
        {
            /// <summary>
            /// The singular <see cref="IOperand"/> of this instruction.
            /// </summary>
            public IOperand Operand => this.Operands[0];
        }

        /// <summary>
        /// Base-class for jump instructions.
        /// </summary>
        public abstract record JumpInstruction(IOperand Target, string? Comment = null)
            : InstructionBase(new[] { Target }, Comment)
        {
            /// <summary>
            /// The target for this jump.
            /// </summary>
            public IOperand Target => this.Operands[0];
        }

        /// <summary>
        /// Base-class for arithmetic instructions.
        /// </summary>
        [Validator(typeof(ArithmeticInstructionValidator))]
        public abstract record ArithmeticInstruction(IOperand Destination, IOperand Source, string? Comment = null)
            : InstructionBase(new[] { Destination, Source }, Comment)
        {
            /// <summary>
            /// The <see cref="IOperand"/> where the result is written. Also acts as the left-hand side operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The right-hand side <see cref="IOperand"/>.
            /// </summary>
            public IOperand Source => this.Operands[1];
        }

        /* Actual instructions */

        /// <summary>
        /// LEAVE instruction.
        /// </summary>
        public record Leave(string? Comment = null) : ZeroOperandInstruction(Comment);

        /// <summary>
        /// RET instruction.
        /// </summary>
        public record Ret(string? Comment = null) : ZeroOperandInstruction(Comment);

        /// <summary>
        /// PUSH instruction.
        /// </summary>
        public record Push(IOperand Operand, string? Comment = null) : OneOperandInstruction(Operand, Comment);

        /// <summary>
        /// ADD instruction.
        /// </summary>
        public record Add(IOperand Destination, IOperand Source, string? Comment = null)
            : ArithmeticInstruction(Destination, Source, Comment);

        /// <summary>
        /// SUB instruction.
        /// </summary>
        public record Sub(IOperand Destination, IOperand Source, string? Comment = null)
            : ArithmeticInstruction(Destination, Source, Comment);

        // NOTE: For now we treat MOV as an arithmetic instruction, maybe it's good enough

        /// <summary>
        /// MOV instruction.
        /// </summary>
        public record Mov(IOperand Destination, IOperand Source, string? Comment = null)
            : ArithmeticInstruction(Destination, Source, Comment);

        /// <summary>
        /// JMP instruction.
        /// </summary>
        public record Jmp(IOperand Target, string? Comment = null) : JumpInstruction(Target, Comment);

        /// <summary>
        /// JLE (jump if less or equal) instruction.
        /// </summary>
        public record Jle(IOperand Target, string? Comment = null) : JumpInstruction(Target, Comment);

        /// <summary>
        /// CMP instruction.
        /// </summary>
        [Validator(typeof(ArithmeticInstructionValidator))]
        public record Cmp(IOperand Left, IOperand Right, string? Comment = null)
            : InstructionBase(new[] { Left, Right }, Comment)
        {
            /// <summary>
            /// The left <see cref="IOperand"/>.
            /// </summary>
            public IOperand Left => this.Operands[0];

            /// <summary>
            /// The right <see cref="IOperand"/>.
            /// </summary>
            public IOperand Right => this.Operands[1];
        }
    }
}
