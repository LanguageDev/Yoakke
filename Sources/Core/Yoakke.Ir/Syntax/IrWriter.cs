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
using AttributeTargets = Yoakke.Ir.Model.Attributes.AttributeTargets;

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

        private readonly Context context;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrWriter"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Context"/> for the IR.</param>
        /// <param name="underlying">The underlying <see cref="TextWriter"/> to write to.</param>
        public IrWriter(Context context, TextWriter underlying)
        {
            this.context = context;
            this.Underlying = underlying;
        }

        /// <summary>
        /// Writes an <see cref="Assembly"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to write.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteAssembly(Assembly assembly)
        {
            this.WriteAttributes(assembly, prefix: string.Empty, printTargetSpec: true);
            this.Underlying.WriteLine();
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
            this.Underlying.Write($"procedure {procedure.Name}()");
            this.WriteAttributes(procedure, printTargetSpec: true);
            this.Underlying.WriteLine(':');
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
            this.Underlying.Write($"block {basicBlock.Name}");
            this.WriteAttributes(basicBlock);
            this.Underlying.WriteLine(':');
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
            var syntax = this.context.GetInstructionSyntax(instruction.GetType());
            this.Underlying.Write(syntax.Name);
            syntax.Print(instruction, this.Underlying);
            if (instruction.GetAttributes().Any()) this.WriteAttributes(instruction);
            this.Underlying.WriteLine();
            return this;
        }

        /// <summary>
        /// Writes the attribute list for an <see cref="IReadOnlyAttributeTarget"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="attributeTarget">The <see cref="IReadOnlyAttributeTarget"/> to write the attributes for.</param>
        /// <param name="prefix">The prefix to print before the attribute list (and before the braces).</param>
        /// <param name="printBrackets">True, if the brackets should be printed.</param>
        /// <param name="printTargetSpec">True, if the target specifier should be printed.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public IrWriter WriteAttributes(
            IReadOnlyAttributeTarget attributeTarget,
            string prefix = " ",
            bool printBrackets = true,
            bool printTargetSpec = false)
        {
            this.Underlying.Write(prefix);
            if (printBrackets) this.Underlying.Write('[');
            if (printTargetSpec)
            {
                this.Underlying.Write(GetAttributeTargetName(attributeTarget.Flag));
                this.Underlying.Write(": ");
            }
            var first = true;
            foreach (var attr in attributeTarget.GetAttributes())
            {
                if (!first) this.Underlying.Write(", ");
                first = false;
                this.WriteAttribute(attr);
            }
            if (printBrackets) this.Underlying.Write(']');
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

        private static string GetAttributeTargetName(Model.Attributes.AttributeTargets target) => target switch
        {
            AttributeTargets.Assembly => "assembly",
            AttributeTargets.BasicBlock => "block",
            AttributeTargets.Instruction => "instruction",
            AttributeTargets.Parameter => "parameter",
            AttributeTargets.Procedure => "procedure",
            AttributeTargets.ReturnValue => "return",
            AttributeTargets.TypeDefinition => "type",
            AttributeTargets.TypeField => "field",
            _ => throw new ArgumentOutOfRangeException(nameof(target)),
        };
    }
}
