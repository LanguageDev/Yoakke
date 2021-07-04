// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Writers
{
    /// <summary>
    /// The different x86 syntax flavors.
    /// </summary>
    public enum SyntaxFlavor
    {
        /// <summary>
        /// Intel syntax with destination, source ordering.
        /// </summary>
        Intel,

        /// <summary>
        /// AT&T syntax with source, destination ordering.
        /// </summary>
        ATnT,
    }
}
