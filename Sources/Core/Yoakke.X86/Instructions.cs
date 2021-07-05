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
    /// A wrapper class for all instructions.
    /// </summary>
    public static class Instructions
    {
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

        public class Add : ArithmeticInstruction
        {
            public Add(IOperand destination, IOperand source, string? comment = null)
                : base(destination, source, comment)
            {
            }
        }
    }
}
