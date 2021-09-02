// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Ir.Model.Builders
{
    /// <summary>
    /// A builder for <see cref="Assembly"/>s.
    /// </summary>
    public class AssemblyBuilder : Assembly
    {
        /// <summary>
        /// Builds a copy <see cref="Assembly"/> of this builder.
        /// </summary>
        /// <returns>The built <see cref="Assembly"/>.</returns>
        public Assembly Build() => new()
        {
            Procedures = this.Procedures.Values.ToDictionary(p => p.Name),
        };
    }
}
