// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model.Types;

namespace Yoakke.Ir.Model.Values
{
    /// <summary>
    /// Represents a constant that depends on the size of its type argument in bits.
    /// </summary>
    public record SizeOf(IType Type) : IConstant
    {
        /// <inheritdoc/>
        public override string ToString() => $"size_of({this.Type})";
    }
}
