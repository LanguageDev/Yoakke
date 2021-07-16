// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Generator
{
    /// <summary>
    /// The type of byte-match for the subnode.
    /// </summary>
    public enum MatchType
    {
        /// <summary>
        /// Requires a matching opcode.
        /// </summary>
        Opcode,

        /// <summary>
        /// Requires a matching prefix.
        /// </summary>
        Prefix,

        /// <summary>
        /// The ModRM register extends the opcode.
        /// </summary>
        ModRmReg,
    }
}
