// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// A compilation unit that can be read and written.
    /// </summary>
    public interface IAssembly : IReadOnlyAssembly
    {
        /// <summary>
        /// The <see cref="IProcedure"/>s in this <see cref="IAssembly"/>.
        /// </summary>
        public new IDictionary<string, IProcedure> Procedures { get; }
    }
}
