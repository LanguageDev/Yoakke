// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// A default implementation of <see cref="IBasicBlock"/>.
    /// </summary>
    public class BasicBlock : IBasicBlock
    {
        private readonly List<Instruction> instructions = new();

        /// <inheritdoc/>
        public IProcedure Procedure { get; }

        /// <inheritdoc/>
        IReadOnlyProcedure IReadOnlyBasicBlock.Procedure => this.Procedure;

        /// <inheritdoc/>
        public IList<Instruction> Instructions => this.instructions;

        /// <inheritdoc/>
        IReadOnlyList<Instruction> IReadOnlyBasicBlock.Instructions => this.instructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBlock"/> class.
        /// </summary>
        /// <param name="procedure">The <see cref="IProcedure"/> this <see cref="BasicBlock"/> belongs to.</param>
        public BasicBlock(IProcedure procedure)
        {
            this.Procedure = procedure;
        }
    }
}
