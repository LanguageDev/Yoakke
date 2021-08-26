// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model.Values;

namespace Yoakke.Ir.Model.Types
{
    /// <summary>
    /// Represents a floating-point value.
    /// </summary>
    public record Float(IConstant Bits) : IType
    {
        /// <inheritdoc/>
        public override string ToString() => $"f{{{this.Bits}}}";
    }
}
