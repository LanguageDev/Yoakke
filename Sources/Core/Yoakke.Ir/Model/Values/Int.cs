// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ir.Model.Values
{
    /// <summary>
    /// An integer constant.
    /// </summary>
    public record Int(int Value) : IConstant
    {
        /// <inheritdoc/>
        public override string ToString() => this.Value.ToString();
    }
}
