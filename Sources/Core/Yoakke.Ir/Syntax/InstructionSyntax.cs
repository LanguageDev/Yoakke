// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// A generic <see cref="IInstructionSyntax"/> implementation with delegates.
    /// </summary>
    /// <typeparam name="TInstruction">The exact, handled <see cref="Instruction"/> type.</typeparam>
    public class InstructionSyntax<TInstruction> : IInstructionSyntax
        where TInstruction : Instruction
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public System.Type Type => typeof(TInstruction);

        private readonly Func<IrParser, TInstruction> parse;
        private readonly Action<TInstruction, IrWriter> print;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionSyntax{TInstruction}"/> class.
        /// </summary>
        /// <param name="name">The name of the handled instruction.</param>
        /// <param name="parse">The parser function.</param>
        /// <param name="print">The print function.</param>
        public InstructionSyntax(string name, Func<IrParser, TInstruction> parse, Action<TInstruction, IrWriter> print)
        {
            this.Name = name;
            this.parse = parse;
            this.print = print;
        }

        /// <inheritdoc/>
        public Instruction Parse(IrParser parser) => this.parse(parser);

        /// <inheritdoc/>
        public void Print(Instruction instruction, IrWriter writer) => this.print((TInstruction)instruction, writer);
    }
}
