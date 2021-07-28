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
    /// The interface for all IR instructions.
    /// </summary>
    public interface IInstruction
    {
        /// <summary>
        /// True, if this is some kind of branching instruction.
        /// </summary>
        public bool IsBranch { get; }
    }
}
