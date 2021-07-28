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
    /// An IR procedure that can only be read.
    /// </summary>
    public interface IReadOnlyProcedure
    {
        /// <summary>
        /// The logical name of this <see cref="IReadOnlyProcedure"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="IReadOnlyBasicBlock"/>s this <see cref="IReadOnlyProcedure"/> consists of.
        /// </summary>
        public IReadOnlyList<IReadOnlyBasicBlock> BasicBlocks { get; }
    }
}
