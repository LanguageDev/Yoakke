// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// An IR procedure that can only be read.
    /// </summary>
    public interface IReadOnlyProcedure
    {
        /// <summary>
        /// The exact <see cref="Type"/> of this <see cref="IReadOnlyProcedure"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The logical name of this <see cref="IReadOnlyProcedure"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="Parameter"/>s this <see cref="IReadOnlyProcedure"/> takes.
        /// </summary>
        public IReadOnlyList<Parameter> Parameters { get; }

        /// <summary>
        /// The <see cref="Model.Type"/> this <see cref="IReadOnlyProcedure"/> returns.
        /// </summary>
        public Type Return { get; }

        /// <summary>
        /// The <see cref="Local"/> allocations in this <see cref="IReadOnlyProcedure"/>.
        /// </summary>
        public IReadOnlyList<Local> Locals { get; }

        /// <summary>
        /// The <see cref="IReadOnlyBasicBlock"/>s this <see cref="IReadOnlyProcedure"/> consists of.
        /// </summary>
        public IReadOnlyList<IReadOnlyBasicBlock> BasicBlocks { get; }
    }
}
