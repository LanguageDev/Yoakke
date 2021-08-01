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
    /// Represents a single, (usually) stack-allocated local variable.
    /// </summary>
    public class Local
    {
        /// <summary>
        /// The <see cref="Ir.Type"/> of the <see cref="Local"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Local"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Ir.Type"/> of the local.</param>
        public Local(Type type)
        {
            this.Type = type;
        }
    }
}
