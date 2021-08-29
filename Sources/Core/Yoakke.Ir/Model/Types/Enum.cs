// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Ir.Model.Types
{
    /// <summary>
    /// A set of identifiers. Used for attribute parameters.
    /// </summary>
    public record Enum(ISet<string> Values) : IType
    {
        /// <inheritdoc/>
        public IType Type => Types.Type.Instance;

        /// <inheritdoc/>
        public override string ToString() => $"enum{{{string.Join(", ", this.Values)}}}";
    }
}
