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
        /// <summary>
        /// Utility to create a new <see cref="LabelRef"/>, before the <see cref="Label"/> is ever
        /// added to the code.
        /// </summary>
        /// <param name="name">The name of the <see cref="Label"/> to create.</param>
        /// <param name="label">The created <see cref="Label"/> gets written here, so it can be added
        /// to the <see cref="Assembly"/> at a later point.</param>
        /// <returns>A new <see cref="LabelRef"/>, that references <paramref name="label"/>.</returns>
        public static LabelRef Forward(string name, out Label label)
        {
            label = new Label(name);
            return new(label);
        }

        public bool IsMemory => true;

        public DataWidth? GetSize() => null;

        public DataWidth GetSize(AssemblyContext context) => context.AddressSize;

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
