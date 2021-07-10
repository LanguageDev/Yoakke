// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// A builder for an x86 <see cref="Assembly"/>.
    /// </summary>
    public class AssemblyBuilder
    {
        // TODO: Sections?
        private readonly List<ICodeElement> elements = new();
        private int position;

        /// <summary>
        /// Returns a new <see cref="X86.Assembly"/> constructed from the contents of this <see cref="AssemblyBuilder"/>.
        /// </summary>
        public Assembly Assembly => new()
        {
            Elements = this.elements.ToList(),
        };

        /// <summary>
        /// The current position of the builder.
        /// </summary>
        public int Position
        {
            get => this.position;
            set
            {
                if (value < 0 || value > this.elements.Count) throw new ArgumentOutOfRangeException(nameof(value));
                this.position = value;
            }
        }

        /// <summary>
        /// Sets the current position of the builder to the given value.
        /// </summary>
        /// <param name="offset">The offset of the position to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">The <see cref="SeekOrigin"/> to seek relative to.</param>
        /// <returns>The new position of the builder.</returns>
        public int Seek(int offset, SeekOrigin origin)
        {
            this.Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => this.Position + offset,
                SeekOrigin.End => this.Position - offset,
                _ => throw new ArgumentException($"invalid {nameof(SeekOrigin)}", nameof(origin)),
            };
            return this.Position;
        }

        /// <summary>
        /// Writes an <see cref="ICodeElement"/> to the current <see cref="Position"/>.
        /// </summary>
        /// <param name="element">The <see cref="ICodeElement"/> to write.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Write(ICodeElement element)
        {
            this.elements.Insert(this.position++, element);
            return this;
        }

        /* Adding a label */

        /// <summary>
        /// Adds a new <see cref="X86.Label"/> with the given name to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="name">The name for the <see cref="X86.Label"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(string name) => this.Label(new Label(name));

        /// <summary>
        /// Adds a new <see cref="X86.Label"/> with the given name to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="name">The name for the <see cref="X86.Label"/> to add.</param>
        /// <param name="labelReference">A <see cref="Operands.LabelRef"/> gets written here, that references
        /// the created label, so you can use it as an operand target later.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(string name, out LabelRef labelReference) =>
            this.Label(new Label(name), out labelReference);

        /// <summary>
        /// Adds a <see cref="X86.Label"/> to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="label">The <see cref="X86.Label"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(Label label) => this.Label(label, out var _);

        /// <summary>
        /// Adds a <see cref="X86.Label"/> to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="label">The <see cref="X86.Label"/> to add.</param>
        /// <param name="labelReference">A <see cref="Operands.LabelRef"/> gets written here, that references
        /// <paramref name="label"/>, so you can use it as an operand target later.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(Label label, out LabelRef labelReference)
        {
            labelReference = new(label);
            return this.Write(label);
        }

        /* Instructions */

        /// <summary>
        /// Writes a PUSH instruction.
        /// </summary>
        /// <param name="op">The pushed operand.</param>
        /// <param name="comment">The optional instruction comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Push(IOperand op, string? comment = null) =>
            this.Write(new Instructions.Push(op, comment));

        /// <summary>
        /// Writes an ADD instruction.
        /// </summary>
        /// <param name="dest">The operand to add to.</param>
        /// <param name="src">The operand to add.</param>
        /// <param name="comment">The optional instruction comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Add(IOperand dest, IOperand src, string? comment = null) =>
            this.Write(new Instructions.Add(dest, src, comment));

        /// <summary>
        /// Writes a SUB instruction.
        /// </summary>
        /// <param name="dest">The operand to subtract from.</param>
        /// <param name="src">The operand to subtract.</param>
        /// <param name="comment">The optional instruction comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sub(IOperand dest, IOperand src, string? comment = null) =>
            this.Write(new Instructions.Sub(dest, src, comment));

        /// <summary>
        /// Writes a MOV instruction.
        /// </summary>
        /// <param name="dest">The operand to move to.</param>
        /// <param name="src">The operand to move.</param>
        /// <param name="comment">The optional instruction comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mov(IOperand dest, IOperand src, string? comment = null) =>
            this.Write(new Instructions.Mov(dest, src, comment));

        /// <summary>
        /// Writes an unconditional JMP instruction.
        /// </summary>
        /// <param name="target">The target operant to jump to.</param>
        /// <param name="comment">The optional instruction comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jmp(IOperand target, string? comment = null) =>
            this.Write(new Instructions.Jmp(target, comment));
    }
}
