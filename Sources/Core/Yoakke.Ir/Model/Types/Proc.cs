// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ir.Model.Types
{
    /// <summary>
    /// Represents a procedure type.
    /// </summary>
    public record Proc(IType Parameters, IType Return) : IType
    {
        /// <inheritdoc/>
        public IType Type => Types.Type.Instance;

        /// <inheritdoc/>
        public override string ToString() => $"proc({string.Join(", ", this.Parameters)}) {this.Return}";
    }
}
