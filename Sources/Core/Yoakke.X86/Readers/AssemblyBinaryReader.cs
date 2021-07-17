// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Utilities;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Readers
{
    /// <summary>
    /// A reader that reads assembly instructions from binary code.
    /// </summary>
    public class AssemblyBinaryReader
    {
        /// <summary>
        /// The underlying <see cref="BinaryReader"/>.
        /// </summary>
        public BinaryReader Underlying { get; }

        private readonly RingBuffer<byte> peekBuffer;
        private int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBinaryReader"/> class.
        /// </summary>
        /// <param name="reader">The underlying <see cref="BinaryReader"/> to read from.</param>
        public AssemblyBinaryReader(BinaryReader reader)
        {
            this.Underlying = reader;
            this.peekBuffer = new();
        }

        /// <summary>
        /// Reads the next <see cref="IInstruction"/> from the <see cref="Underlying"/> reader.
        /// </summary>
        /// <param name="length">The number of read bytes will be written here, so the caller can know the exact
        /// byte-length of the parser <see cref="IInstruction"/>.</param>
        /// <returns>The parsed <see cref="IInstruction"/>.</returns>
        public IInstruction ReadNext(out int length)
        {
            #region Generated

            #endregion Generated

            throw new NotImplementedException();
        }

        private byte ParseByte()
        {
            // NOTE: We only need to look ahead one, hopefully this is correct
            if (this.peekBuffer.Count <= this.offset) this.peekBuffer.AddBack(this.Underlying.ReadByte());

            var result = this.peekBuffer[this.offset];
            ++this.offset;
            return result;
        }

        private void UnparseByte() => --this.offset;

        private IOperand ParseImmediate(DataWidth width) => throw new NotImplementedException();

        private IOperand ParseRM(byte modrm, DataWidth width) => throw new NotImplementedException();

        private int Commit()
        {
            var result = this.offset;
            for (var i = 0; i < this.offset; ++i) this.peekBuffer.RemoveFront();
            this.offset = 0;
            return result;
        }

        private static IOperand FromRegisterIndex(int index, DataWidth width) => throw new NotImplementedException();
    }
}
