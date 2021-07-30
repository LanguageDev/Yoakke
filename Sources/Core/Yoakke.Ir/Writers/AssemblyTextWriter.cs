// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir.Writers
{
    /// <summary>
    /// Basic class for writing IR assembly.
    /// </summary>
    public class AssemblyTextWriter
    {
        /// <summary>
        /// The underlying <see cref="StringBuilder"/> this <see cref="AssemblyTextWriter"/> writes to.
        /// </summary>
        public StringBuilder Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTextWriter"/> class.
        /// </summary>
        /// <param name="result">The <see cref="StringBuilder"/> to write the code to.</param>
        public AssemblyTextWriter(StringBuilder result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTextWriter"/> class.
        /// </summary>
        public AssemblyTextWriter()
            : this(new StringBuilder())
        {
        }

        /// <summary>
        /// Writes a character to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(char c)
        {
            this.Result.Append(c);
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(string str)
        {
            this.Result.Append(str);
            return this;
        }

        /// <summary>
        /// Starts a new line for the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine()
        {
            this.Result.AppendLine();
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(string str) => this.Write(str).WriteLine();

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(object? obj) => this.Write(obj?.ToString() ?? "null");

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(object? obj) => this.Write(obj).WriteLine();

        /// <summary>
        /// Writes an <see cref="IReadOnlyBasicBlock"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="basicBlock">The <see cref="IReadOnlyBasicBlock"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(IReadOnlyBasicBlock basicBlock)
        {
            // TODO: Blocks need an identifier
            this.Write("block ???:");
            foreach (var ins in basicBlock.Instructions) this.WriteLine().Write("  ").Write(ins);
            return this;
        }

        /// <summary>
        /// Writes an <see cref="IReadOnlyBasicBlock"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="basicBlock">The <see cref="IReadOnlyBasicBlock"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(IReadOnlyBasicBlock basicBlock) => this.Write(basicBlock).WriteLine();

        /// <summary>
        /// Writes an <see cref="IInstruction"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(IInstruction instruction)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes an <see cref="IInstruction"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(IInstruction instruction) => this.Write(instruction).WriteLine();

        /// <summary>
        /// Writes a <see cref="Value"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(Value value) => value switch
        {
            Value.Arg a => this.Write("arg(").Write(a.Index).Write(')'),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes a <see cref="Value"/> to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(Value value) => this.Write(value).WriteLine();

        /// <summary>
        /// Writes a <see cref="Type"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(Type type) => type switch
        {
            Type.Void => this.Write("void"),
            Type.Int i => this.Write(i.Signed ? 'i' : 'u').Write(i.Bits),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes a <see cref="Type"/> to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(Type type) => this.Write(type).WriteLine();
    }
}
