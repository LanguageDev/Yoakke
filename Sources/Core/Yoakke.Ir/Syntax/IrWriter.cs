// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// A writer to write IR code as text.
    /// </summary>
    public class IrWriter
    {
        /// <summary>
        /// The underlying <see cref="TextWriter"/> to write to.
        /// </summary>
        public TextWriter Underlying { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrWriter"/> class.
        /// </summary>
        /// <param name="underlying">The underlying <see cref="TextWriter"/> to write to.</param>
        public IrWriter(TextWriter underlying)
        {
            this.Underlying = underlying;
        }

        /// <summary>
        /// Writes an <see cref="Assembly"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter Write(Assembly assembly)
        {
            foreach (var procedure in assembly.Procedures.Values) this.Write(procedure);
            return this;
        }

        /// <summary>
        /// Writes a <see cref="Procedure"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="procedure">The <see cref="Procedure"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter Write(Procedure procedure)
        {
            this.Underlying.WriteLine($"proc {procedure.Name}():");
            this.Write(procedure.Entry);
            foreach (var basicBlock in procedure.BasicBlocks.Except(new[] { procedure.Entry })) this.Write(basicBlock);
            return this;
        }

        /// <summary>
        /// Writes a <see cref="BasicBlock"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="basicBlock">The <see cref="BasicBlock"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter Write(BasicBlock basicBlock)
        {
            this.Underlying.WriteLine($"block {basicBlock.Name}:");
            foreach (var instruction in basicBlock.Instructions)
            {
                this.Underlying.Write("  ");
                this.Write(instruction);
            }
            return this;
        }

        /// <summary>
        /// Writes an <see cref="Instruction"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter Write(Instruction instruction)
        {
            var text = instruction switch
            {
                Instruction.Nop => "nop",
                Instruction.Ret => "ret",
                _ => throw new ArgumentOutOfRangeException(nameof(instruction)),
            };
            this.Underlying.WriteLine(text);
            return this;
        }
    }
}
