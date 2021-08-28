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
    /// An integer constant.
    /// </summary>
    public record Int(int Value) : IConstant
    {
        /// <inheritdoc/>
        // TODO: Hardcoded
        public IType Type => new Types.Int(new Int(32));

        /// <inheritdoc/>
        public override string ToString() => this.Value.ToString();
    }
}
