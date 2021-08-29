// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Ir.Model.Types;

namespace Yoakke.Ir.Model.Values
{
    /// <summary>
    /// A single alternative in an enum.
    /// </summary>
    public record EnumValue(Enum Enum, string Value) : IConstant
    {
        /// <inheritdoc/>
        public IType Type => this.Enum;
    }
}
