// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// A default implementation for <see cref="IProcedure"/>.
    /// </summary>
    public class Procedure : IProcedure
    {
        private readonly List<Parameter> parameters = new();
        private readonly List<IBasicBlock> basicBlocks = new();
        private readonly List<Local> locals = new();

        /// <inheritdoc/>
        public Type Type =>
            new Type.Proc(this.Return, this.parameters.Select(p => p.Type).ToList().AsValue());

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<Parameter> Parameters => this.parameters;

        /// <inheritdoc/>
        IReadOnlyList<Parameter> IReadOnlyProcedure.Parameters => this.parameters;

        /// <inheritdoc/>
        public Type Return { get; set; } = Type.Void.Instance;

        /// <inheritdoc/>
        Type IReadOnlyProcedure.Return => this.Return;

        /// <inheritdoc/>
        public IList<Local> Locals => this.locals;

        /// <inheritdoc/>
        IReadOnlyList<Local> IReadOnlyProcedure.Locals => this.locals;

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
