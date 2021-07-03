// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Instructions;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Writers
{
    /// <summary>
    /// Base class for writers that emit textual assembly code.
    /// </summary>
    public abstract class AssemblyWriter
    {
        /// <summary>
        /// The underlying <see cref="TextWriter"/> this <see cref="AssemblyWriter"/> writes to.
        /// </summary>
        public TextWriter TextWriter { get; }

        protected AssemblyWriter(TextWriter textWriter)
        {
            this.TextWriter = textWriter;
        }

        protected virtual void WriteElement(IAssemblyElement element)
        {
            switch (element)
            {
            case IInstruction instruction:
                this.Write(instruction);
                break;

            case Label label:
                this.Write(label);
                break;

            case Comment comment:
                this.Write(comment);
                break;

            default: throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Writes the given <see cref="Assembly"/> to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to write.</param>
        public abstract void Write(Assembly assembly);

        /// <summary>
        /// Writes the given <see cref="IInstruction"/> to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to write.</param>
        public abstract void Write(IInstruction instruction);

        /// <summary>
        /// Writes the given <see cref="IOperand"/> to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="operand">The <see cref="IOperand"/> to write.</param>
        public abstract void Write(IOperand operand);

        /// <summary>
        /// Writes the given <see cref="Label"/> to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to write.</param>
        public abstract void Write(Label label);

        /// <summary>
        /// Writes the given <see cref="Comment"/> to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="comment">The <see cref="Comment"/> to write.</param>
        public abstract void Write(Comment comment);

        protected abstract void WriteInlineComment(string comment);
    }
}
