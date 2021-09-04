// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Attributes;

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
        public IrWriter WriteAssembly(Assembly assembly)
        {
            foreach (var procedure in assembly.Procedures.Values) this.WriteProcedure(procedure);
            return this;
        }

        /// <summary>
        /// Writes a <see cref="Procedure"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="procedure">The <see cref="Procedure"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteProcedure(Procedure procedure)
        {
            this.Underlying.WriteLine($"proc {procedure.Name}():");
            this.WriteBasicBlock(procedure.Entry);
            foreach (var basicBlock in procedure.BasicBlocks.Except(new[] { procedure.Entry })) this.WriteBasicBlock(basicBlock);
            return this;
        }

        /// <summary>
        /// Writes a <see cref="BasicBlock"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="basicBlock">The <see cref="BasicBlock"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteBasicBlock(BasicBlock basicBlock)
        {
            this.Underlying.WriteLine($"block {basicBlock.Name}:");
            foreach (var instruction in basicBlock.Instructions)
            {
                this.Underlying.Write("  ");
                this.WriteInstruction(instruction);
            }
            return this;
        }

        /// <summary>
        /// Writes an <see cref="Instruction"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteInstruction(Instruction instruction)
        {
            // Instruction text
            var text = instruction switch
            {
                Instruction.Nop => "nop",
                Instruction.Ret => "ret",
                _ => throw new ArgumentOutOfRangeException(nameof(instruction)),
            };
            this.Underlying.Write(text);

            // Attributes
            if (instruction.GetAttributes().Any())
            {
                this.Underlying.Write(' ');
                this.WriteAttributes(instruction);
            }

            // Newline
            this.Underlying.WriteLine();
            return this;
        }

        /// <summary>
        /// Writes the attribute list for an <see cref="IAttributeTarget"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="attributeTarget">The <see cref="IAttributeTarget"/> to write the attributes for.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteAttributes(IAttributeTarget attributeTarget)
        {
            this.Underlying.Write('[');
            var first = true;
            foreach (var attr in attributeTarget.GetAttributes())
            {
                if (!first) this.Underlying.Write(", ");
                first = false;
                this.WriteAttribute(attr);
            }
            this.Underlying.Write(']');
            return this;
        }

        /// <summary>
        /// Writes an <see cref="IAttribute"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="attribute">The <see cref="IAttribute"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteAttribute(IAttribute attribute)
        {
            // TODO
            if (attribute.Arguments.Count > 0) throw new NotImplementedException("todo");

            this.Underlying.Write(attribute.Definition.Name);
            return this;
        }
    }
}
