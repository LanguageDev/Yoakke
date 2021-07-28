// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

namespace Yoakke.Ir
{
    /// <summary>
    /// A default implementation of <see cref="IAssembly"/>.
    /// </summary>
    public class Assembly : IAssembly
    {
        private readonly Dictionary<string, IProcedure> procedures;
        private readonly ReadOnlyDictionary<string, IProcedure, IReadOnlyProcedure> readOnlyProcedures;

        /// <inheritdoc/>
        public IDictionary<string, IProcedure> Procedures => this.procedures;

        /// <inheritdoc/>
        IReadOnlyDictionary<string, IReadOnlyProcedure> IReadOnlyAssembly.Procedures => this.readOnlyProcedures;

        /// <summary>
        /// Initializes a new instance of the <see cref="Assembly"/> class.
        /// </summary>
        public Assembly()
        {
            this.procedures = new();
            this.readOnlyProcedures = new(this.procedures);
        }
    }
}
