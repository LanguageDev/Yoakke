// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// A generic <see cref="IInstructionSyntax"/> implementation with delegates.
    /// </summary>
    /// <typeparam name="TInstruction">The exact, handled <see cref="Instruction"/> type.</typeparam>
    public class InstructionSyntax<TInstruction> : IInstructionSyntax
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public System.Type Type => typeof(TInstruction);

        private readonly Func<IrParser, Instruction> parse;
        private readonly Action<Instruction, TextWriter> print;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionSyntax{TInstruction}"/> class.
        /// </summary>
        /// <param name="name">The name of the handled instruction.</param>
        /// <param name="parse">The parser function.</param>
        /// <param name="print">The print function.</param>
        public InstructionSyntax(string name, Func<IrParser, Instruction> parse, Action<Instruction, TextWriter> print)
        {
            this.Name = name;
            this.parse = parse;
            this.print = print;
        }

        /// <inheritdoc/>
        public Instruction Parse(IrParser parser) => this.parse(parser);

        /// <inheritdoc/>
        public void Print(Instruction instruction, TextWriter writer) => this.print(instruction, writer);
    }
}
