// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// An IR procedure that can be read and written.
    /// </summary>
    public interface IProcedure : IReadOnlyProcedure
    {
        /// <summary>
        /// The <see cref="Parameter"/>s this <see cref="IProcedure"/> takes.
        /// </summary>
        public new IList<Parameter> Parameters { get; }

        /// <summary>
        /// The <see cref="Type"/> this <see cref="IProcedure"/> returns.
        /// </summary>
        public new Type Return { get; set; }

        /// <summary>
        /// The <see cref="Local"/> allocations in this <see cref="IProcedure"/>.
        /// </summary>
        public new IList<Local> Locals { get; }

        /// <summary>
        /// The <see cref="IBasicBlock"/>s this <see cref="IProcedure"/> consists of.
        /// </summary>
        public new IList<IBasicBlock> BasicBlocks { get; }
    }
}
