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
    /// A constant operand.
    /// </summary>
    public readonly struct Constant : IOperand
    {
        /// <summary>
        /// The value of this <see cref="Constant"/>.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="value">The value object.</param>
        private Constant(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public Constant(int value)
            : this((object)value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> struct.
        /// </summary>
        /// <param name="value">The long value.</param>
        public Constant(long value)
            : this((object)value)
        {
        }
    }
}
