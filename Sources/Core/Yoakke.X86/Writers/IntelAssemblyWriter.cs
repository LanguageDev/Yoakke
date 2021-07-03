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
    /// An <see cref="AssemblyWriter"/> that only implements <see cref="IInstruction"/> and <see cref="IOperand"/>
    /// writing in the Intel Assembly syntax.
    /// </summary>
    public class IntelAssemblyWriter : AssemblyWriter
    {
        public IntelAssemblyWriter(TextWriter textWriter)
            : base(textWriter)
        {
        }

        public override void Write(IInstruction instruction) => throw new NotImplementedException();

        public override void Write(IOperand operand) => throw new NotImplementedException();

        public override void Write(Assembly assembly) => throw new NotSupportedException();

        public override void Write(Label label) => throw new NotSupportedException();

        public override void Write(Comment comment) => throw new NotSupportedException();
    }
}
