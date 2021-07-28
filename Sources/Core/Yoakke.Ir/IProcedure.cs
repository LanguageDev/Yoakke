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
    /// An IR procedure that can be read and written.
    /// </summary>
    public interface IProcedure : IReadOnlyProcedure
    {
        /// <summary>
        /// The <see cref="IBasicBlock"/>s this <see cref="IProcedure"/> consists of.
        /// </summary>
        public new IList<IBasicBlock> BasicBlocks { get; }
    }
}
