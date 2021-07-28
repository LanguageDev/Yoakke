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
    /// A default implementation for <see cref="IProcedure"/>.
    /// </summary>
    public class Procedure : IProcedure
    {
        private readonly List<IBasicBlock> basicBlocks = new();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<IBasicBlock> BasicBlocks => this.basicBlocks;

        /// <inheritdoc/>
        IReadOnlyList<IReadOnlyBasicBlock> IReadOnlyProcedure.BasicBlocks => this.basicBlocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Procedure"/> class.
        /// </summary>
        /// <param name="name">The logical name of this <see cref="Procedure"/>.</param>
        public Procedure(string name)
        {
            this.Name = name;
        }
    }
}
