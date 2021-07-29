// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Platform.X86.Writers
{
    /// <summary>
    /// Writes the assembly code in binary format.
    /// </summary>
    public class AssemblyBinaryWriter
    {
        /// <summary>
        /// The underlying <see cref="BinaryWriter"/> to write to.
        /// </summary>
        public BinaryWriter Underlying { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBinaryWriter"/> class.
        /// </summary>
        /// <param name="writer">The underlying <see cref="BinaryWriter"/> to write to.</param>
        public AssemblyBinaryWriter(BinaryWriter writer)
        {
            this.Underlying = writer;
        }

        /// <summary>
        /// Writes an <see cref="IInstruction"/> to the <see cref="Underlying"/> writer.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to write.</param>
        public void Write(IInstruction instruction)
        {
            throw new NotImplementedException();
        }
    }
}
