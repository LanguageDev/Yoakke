// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ir.Model.Types
{
    /// <summary>
    /// Void type.
    /// </summary>
    public record Void : IType
    {
        /// <inheritdoc/>
        public IType Type => Types.Type.Instance;

        /// <summary>
        /// A singleton instance to use.
        /// </summary>
        public static readonly Void Instance = new();

        /// <inheritdoc/>
        public override string ToString() => "void";
    }
}
