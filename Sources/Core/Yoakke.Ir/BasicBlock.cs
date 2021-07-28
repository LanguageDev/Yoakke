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
        private readonly List<IInstruction> instructions = new();

        /// <inheritdoc/>
        public string? NameHint { get; init; }

        /// <inheritdoc/>
        public IList<IInstruction> Instructions => this.instructions;

        /// <inheritdoc/>
        IReadOnlyList<IInstruction> IReadOnlyBasicBlock.Instructions => this.instructions;
    }
}
