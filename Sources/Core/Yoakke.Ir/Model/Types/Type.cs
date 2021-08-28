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
    /// A type of types.
    /// </summary>
    public record Type : IType
    {
        /// <inheritdoc/>
        IType IConstant.Type => Instance;

        /// <summary>
        /// A singleton instance to use.
        /// </summary>
        public static readonly Type Instance = new();

        /// <inheritdoc/>
        public override string ToString() => "type";
    }
}
