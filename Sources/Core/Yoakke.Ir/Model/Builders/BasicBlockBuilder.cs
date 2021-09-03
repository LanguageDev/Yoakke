// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Ir.Model.Builders
{
    /// <summary>
    /// A builder for <see cref="BasicBlock"/>s.
    /// </summary>
    public class BasicBlockBuilder : BasicBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBlockBuilder"/> class.
        /// </summary>
        /// <param name="name">The suggested name of the basic block.</param>
        public BasicBlockBuilder(string? name = null)
            : base(name)
        {
        }

        /// <summary>
        /// Builds a copy <see cref="BasicBlock"/> of this builder.
        /// </summary>
        /// <returns>The built <see cref="BasicBlock"/>.</returns>
        public BasicBlock Build() => new()
        {
            Name = this.Name,
            Instructions = this.Instructions.ToList(),
        };
    }
}
