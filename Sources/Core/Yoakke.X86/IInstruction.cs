// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// A single x86 instruction.
    /// </summary>
    public interface IInstruction : ICodeElement
    {
        /// <summary>
        /// The <see cref="IOperand"/>s this <see cref="IInstruction"/> needs.
        /// </summary>
        public IEnumerable<IOperand> Operands { get; }

        /// <summary>
        /// Optional inline comment.
        /// </summary>
        public string? Comment { get; }
    }
}
