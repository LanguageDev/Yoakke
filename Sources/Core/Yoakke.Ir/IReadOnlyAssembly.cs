// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// A compilation unit that can only be read.
    /// </summary>
    public interface IReadOnlyAssembly
    {
        /// <summary>
        /// The <see cref="IReadOnlyProcedure"/>s in this <see cref="IReadOnlyAssembly"/>.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyProcedure> Procedures { get; }
    }
}
