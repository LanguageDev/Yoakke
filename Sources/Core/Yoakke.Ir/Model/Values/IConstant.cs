// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Ir.Model.Types;

namespace Yoakke.Ir.Model.Values
{
    /// <summary>
    /// Represents some compile-time constant value.
    /// </summary>
    public interface IConstant
    {
        /// <summary>
        /// The type of this constant.
        /// </summary>
        public IType Type { get; }
    }
}
