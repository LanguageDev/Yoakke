// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// Any value that can be an instruction argument.
    /// </summary>
    public abstract record Value
    {
        /// <summary>
        /// The type of the value.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// A constant value.
        /// </summary>
        public record Constant(Model.Constant Value) : Value
        {
            /// <inheritdoc/>
            public override Type Type => this.Value.Type;
        }

        /// <summary>
        /// The result value of some instruction.
        /// </summary>
        public record Result(Instruction Instruction, string? Name = null) : Value
        {
            /// <inheritdoc/>
            public override Type Type => this.Instruction.ResultType
                                      ?? throw new InvalidOperationException("The instruction produces no values");
        }
    }
}
