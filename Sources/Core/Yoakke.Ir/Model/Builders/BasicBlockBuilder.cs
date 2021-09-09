// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Ir.Model.Builders
{
    /// <summary>
    /// A builder for <see cref="BasicBlock"/>s.
    /// </summary>
    public class BasicBlockBuilder : BasicBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBlockBuilder"/> class.
        /// </summary>
        /// <param name="name">The suggested name of the basic block.</param>
        public BasicBlockBuilder(string? name = null)
            : base(name)
        {
        }

        /// <summary>
        /// Builds a copy <see cref="BasicBlock"/> of this builder.
        /// </summary>
        /// <returns>The built <see cref="BasicBlock"/>.</returns>
        public BasicBlock Build() => new()
        {
            Name = this.Name,
            Instructions = this.Instructions.ToList(),
        };

        /// <summary>
        /// Adds an instruction to this block.
        /// </summary>
        /// <param name="instruction">The instruction to add.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public BasicBlockBuilder WithInstruction(Instruction instruction)
        {
            this.Instructions.Add(instruction);
            return this;
        }

        /// <summary>
        /// Writes a NOP instruction.
        /// </summary>
        /// <returns>This instance, to be able to chain calls.</returns>
        public BasicBlockBuilder Nop() => this.WithInstruction(new Instruction.Nop());

        /// <summary>
        /// Writes a RET instruction.
        /// </summary>
        /// <returns>This instance, to be able to chain calls.</returns>
        public BasicBlockBuilder Ret() => this.WithInstruction(new Instruction.Ret());

        /// <summary>
        /// Writes an ADD instruction.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public BasicBlockBuilder Add(Value left, Value right) => this.WithInstruction(new Instruction.Add(left, right));
    }
}
