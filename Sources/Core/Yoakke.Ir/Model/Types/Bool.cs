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
    /// Represents a set of boolean bit values.
    /// 0 meaning false and 1 meaning true is not guaranteed.
    /// </summary>
    public record Bool(IConstant Bits) : IType
    {
        /// <inheritdoc/>
        public override string ToString() => $"b{{{this.Bits}}}";
    }
}
