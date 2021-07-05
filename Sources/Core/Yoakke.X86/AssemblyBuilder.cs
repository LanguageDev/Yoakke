// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Assembly Assembly => new Assembly
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
        public AssemblyBuilder WriteElement(ICodeElement element)
        {
            this.elements.Insert(this.position++, element);
            return this;
        }

        public AssemblyBuilder Add(IOperand dest, IOperand src, string? comment = null) =>
            this.WriteElement(new Instructions.Add(dest, src, comment));
    }
}
