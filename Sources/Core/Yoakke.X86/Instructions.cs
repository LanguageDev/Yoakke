// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Validator(typeof(OneOperandInstructionValidator))]
        public abstract class OneOperandInstruction : IInstruction
        {
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Operand;
                }
            }

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

        public abstract class JumpInstruction : IInstruction
        {
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Target;
                }
            }

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

        [Validator(typeof(ArithmeticInstructionValidator))]
        public abstract class ArithmeticInstruction : IInstruction
        {
            public IEnumerable<IOperand> Operands
            {
                get
                {
                    yield return this.Destination;
                    yield return this.Source;
                }
            }

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

        public class Push : OneOperandInstruction
        {
            public Push(IOperand operand, string? comment = null)
                : base(operand, comment)
            {
            }
        }

        public class Add : ArithmeticInstruction
        {
            public Add(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        public class Sub : ArithmeticInstruction
        {
            public Sub(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        // NOTE: For now we treat MOV as an arithmetic instruction, maybe it's good enough
        public class Mov : ArithmeticInstruction
        {
            public Mov(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }

        public class Jmp : JumpInstruction
        {
            public Jmp(IOperand target, string? comment = null)
                : base(target, comment)
            {
            }
        }
    }
}
