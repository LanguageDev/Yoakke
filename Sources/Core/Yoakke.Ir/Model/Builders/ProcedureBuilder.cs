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
    /// A builder for <see cref="Procedure"/>s.
    /// </summary>
    public class ProcedureBuilder : Procedure
    {
        /// <summary>
        /// Builds a copy <see cref="Procedure"/> of this builder.
        /// </summary>
        /// <returns>The built <see cref="Procedure"/>.</returns>
        public Procedure Build() => new()
        {
            Name = this.Name,
            Entry = this.Entry,
            BasicBlocks = this.BasicBlocks.ToList(),
        };
    }
}
