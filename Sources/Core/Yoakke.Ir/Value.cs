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
    /// The base for all IR values.
    /// </summary>
    public abstract record Value
    {
        /// <summary>
        /// An argument reference.
        /// </summary>
        public record Arg(int Index) : Value;
    }
}
