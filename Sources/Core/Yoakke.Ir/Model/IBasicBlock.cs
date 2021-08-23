// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// A basic block that can be read and written.
    /// A basic block is a sequence of instructions that only contains branching at the end.
    /// This means that the code can only jump at the start of the block and only the last instruction
    /// can jump elsewhere.
    /// </summary>
    public interface IBasicBlock : IReadOnlyBasicBlock
    {
        /// <summary>
        /// The <see cref="IProcedure"/> this <see cref="IBasicBlock"/> belongs to.
        /// </summary>
        public new IProcedure Procedure { get; }

        /// <summary>
        /// The list of <see cref="Instruction"/>s in this <see cref="IBasicBlock"/>.
        /// </summary>
        public new IList<Instruction> Instructions { get; }
    }
}
