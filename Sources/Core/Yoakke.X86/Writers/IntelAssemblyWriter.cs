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
#if false
    /// <summary>
    /// An <see cref="AssemblyWriter"/> that only implements <see cref="IInstruction"/> and <see cref="IOperand"/>
    /// writing in the Intel Assembly syntax.
    /// </summary>
    public class IntelAssemblyWriter : AssemblyWriter
    {
        public IntelAssemblyWriter(TextWriter textWriter)
            : base(textWriter)
        {
        }

        public override void Write(IInstruction instruction)
        {
            this.TextWriter.Write(instruction.Opcode.ToString().ToLower());

            // Operands
            if (instruction.Operands.Any())
            {
                this.TextWriter.Write(' ');
                var first = true;
                foreach (var operand in instruction.Operands)
                {
                    if (!first) this.TextWriter.Write(", ");
                    first = false;
                    this.Write(operand);
                }
            }

            // Comment
            if (instruction.Comment is not null) this.WriteInlineComment(instruction.Comment);
        }

        public override void Write(IOperand operand)
        {
            switch (operand)
            {
            case Constant constant:
                this.TextWriter.Write(constant.Value);
                break;

            case Register register:
                this.TextWriter.Write(register.Name);
                break;

            case Segment segment:
                this.TextWriter.Write(segment.Name);
                break;

            case Address address:
                this.WriteAddress(address);
                break;

            case Indirect indirect:
                this.TextWriter.Write(indirect.Size.ToString().ToUpper());
                this.TextWriter.Write(" PTR ");
                this.WriteAddress(indirect.Address);
                break;

            default: throw new NotSupportedException();
            }
        }

        public override void Write(Assembly assembly) => throw new NotSupportedException();

        public override void Write(Label label) => throw new NotSupportedException();

        public override void Write(Comment comment) => throw new NotSupportedException();

        protected override void WriteInlineComment(string comment) => throw new NotSupportedException();

        protected virtual void WriteAddress(Address address)
        {
            // Optional segment override
            // All this logic is here so that if we have SegmentSelectorInBrackets, then we get
            // [segment: ...]
            // otherwise
            // segment:[...]
            if (address.Segment is not null)
            {
                if (this.SegmentSelectorInBrackets) this.TextWriter.Write('[');

                this.TextWriter.Write(address.Segment.Value.Name);
                this.TextWriter.Write(':');

                if (this.SegmentSelectorInBrackets) this.TextWriter.Write(' ');
                else this.TextWriter.Write('[');
            }
            else
            {
                this.TextWriter.Write('[');
            }

            var written = false;

            // Write the base first if there is one
            if (address.Base is not null)
            {
                written = true;
                this.TextWriter.Write(address.Base.Value.Name);
            }

            // Then write the scaled index
            if (address.ScaledIndex is not null)
            {
                if (written) this.TextWriter.Write(" + ");
                written = true;
                var (index, scale) = address.ScaledIndex.Value;
                this.TextWriter.Write(index.Name);
                this.TextWriter.Write(" * ");
                this.TextWriter.Write(scale);
            }

            // Finally the displacement
            if (!written)
            {
                // If we haven't written anything, always write displacement
                this.TextWriter.Write(address.Displacement);
            }
            else if (address.Displacement != 0)
            {
                // Otherwise only write it if it's nonzero
                this.TextWriter.Write(" + ");
                this.TextWriter.Write(address.Displacement);
            }

            this.TextWriter.Write(']');
        }
    }
#endif
}
