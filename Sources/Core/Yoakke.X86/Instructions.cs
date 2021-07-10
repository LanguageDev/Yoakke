// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
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
        /// Base-class for single-operand instructions.
        /// </summary>
        [Validator(typeof(OneOperandInstructionValidator))]
        public abstract class OneOperandInstruction : IInstruction
        {
            /// <inheritdoc/>
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Operand;
                }
            }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The single <see cref="IOperand"/> for this instruction.
            /// </summary>
            public IOperand Operand { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="OneOperandInstruction"/> class.
            /// </summary>
            /// <param name="operand">The single <see cref="IOperand"/> for this instruction.</param>
            /// <param name="comment">The optional inline comment.</param>
            public OneOperandInstruction(IOperand operand, string? comment = null)
            {
                this.Operand = operand;
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Base-class for jump instructions.
        /// </summary>
        public abstract class JumpInstruction : IInstruction
        {
            /// <inheritdoc/>
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Target;
                }
            }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The target for this jump.
            /// </summary>
            public IOperand Target { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="JumpInstruction"/> class.
            /// </summary>
            /// <param name="target">The target for this jump.</param>
            /// <param name="comment">The optional inline comment.</param>
            public JumpInstruction(IOperand target, string? comment = null)
            {
                this.Target = target;
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Base-class for arithmetic instructions.
        /// </summary>
        [Validator(typeof(ArithmeticInstructionValidator))]
        public abstract class ArithmeticInstruction : IInstruction
        {
            /// <inheritdoc/>
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Destination;
                    yield return this.Source;
                }
            }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The <see cref="IOperand"/> where the result is written. Also acts as the left-hand side operand.
            /// </summary>
            public IOperand Destination { get; }

            /// <summary>
            /// The right-hand side <see cref="IOperand"/>.
            /// </summary>
            public IOperand Source { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ArithmeticInstruction"/> class.
            /// </summary>
            /// <param name="destination">The <see cref="IOperand"/> where the result is written.
            /// Also acts as the left-hand side operand.</param>
            /// <param name="source">The right-hand side <see cref="IOperand"/>.</param>
            /// <param name="comment">The optional inline comment.</param>
            public ArithmeticInstruction(IOperand destination, IOperand source, string? comment = null)
            {
                this.Destination = destination;
                this.Source = source;
                this.Comment = comment;
            }
        }

        /* Actual instructions */

        /// <summary>
        /// PUSH instruction.
        /// </summary>
        public class Push : OneOperandInstruction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Push"/> class.
            /// </summary>
            /// <param name="operand">The operand to push.</param>
            /// <param name="comment">An optional line-comment.</param>
            public Push(IOperand operand, string? comment = null)
                : base(operand, comment)
            {
            }
        }

        /// <summary>
        /// ADD instruction.
        /// </summary>
        public class Add : ArithmeticInstruction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Add"/> class.
            /// </summary>
            /// <param name="destination">The operand to add to.</param>
            /// <param name="source">The operand to add.</param>
            /// <param name="comment">An optional line-comment.</param>
            public Add(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        /// <summary>
        /// SUB instruction.
        /// </summary>
        public class Sub : ArithmeticInstruction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Sub"/> class.
            /// </summary>
            /// <param name="destination">The operand to subtract from.</param>
            /// <param name="source">The operand to subtract.</param>
            /// <param name="comment">An optional line-comment.</param>
            public Sub(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        // NOTE: For now we treat MOV as an arithmetic instruction, maybe it's good enough

        /// <summary>
        /// MOV instruction.
        /// </summary>
        public class Mov : ArithmeticInstruction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Mov"/> class.
            /// </summary>
            /// <param name="destination">The operand to move to.</param>
            /// <param name="source">The operand to move.</param>
            /// <param name="comment">An optional line-comment.</param>
            public Mov(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        /// <summary>
        /// JMP instruction.
        /// </summary>
        public class Jmp : JumpInstruction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Jmp"/> class.
            /// </summary>
            /// <param name="target">The target operand to jump to.</param>
            /// <param name="comment">An optional line-comment.</param>
            public Jmp(IOperand target, string? comment = null)
                : base(target, comment)
            {
            }
        }
    }
}
