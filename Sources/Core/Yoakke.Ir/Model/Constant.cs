// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// Represents some compile-time constant value.
    /// </summary>
    public abstract record Constant
    {
        /// <summary>
        /// The <see cref="Model.Type"/> of this constant.
        /// </summary>
        public abstract Type Type { get; }
    }
}
