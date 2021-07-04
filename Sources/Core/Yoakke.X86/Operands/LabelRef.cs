// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Operands
{
    /// <summary>
    /// A <see cref="Label"/> reference.
    /// </summary>
    public readonly struct LabelRef : IOperand
    {
        public bool IsMemory => true;

        public DataWidth? Size => null;

        /// <summary>
        /// The referenced <see cref="X86.Label"/>.
        /// </summary>
        public readonly Label Label;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelRef"/> struct.
        /// </summary>
        /// <param name="label">The referenced <see cref="Label"/>.</param>
        public LabelRef(Label label)
        {
            this.Label = label;
        }
    }
}
