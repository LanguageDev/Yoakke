// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Ir.Model.Values;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// An attribute instance.
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        /// The definition of this instance.
        /// </summary>
        public IAttributeDefinition Definition { get; }

        /// <summary>
        /// The arguments passed in for this attribute instance.
        /// </summary>
        public IReadOnlyList<IConstant> Arguments { get; }
    }
}
