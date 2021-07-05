// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// Context for assembly configuration.
    /// </summary>
    public class AssemblyContext
    {
        /// <summary>
        /// Returns a default instance with some sensible defaults.
        /// </summary>
        public static AssemblyContext Default => new();

        /// <summary>
        /// The address size for the current compilation.
        /// </summary>
        public DataWidth AddressSize { get; set; } = DataWidth.Dword;
    }
}
