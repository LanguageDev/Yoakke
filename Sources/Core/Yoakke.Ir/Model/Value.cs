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
    /// The base for all IR values.
    /// </summary>
    public abstract record Value : IInstructionArg
    {
        /// <summary>
        /// The <see cref="Type"/> of this <see cref="Value"/>.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// An argument reference.
        /// </summary>
        public record Argument(Parameter Parameter) : Value
        {
            /// <inheritdoc/>
            public override Type Type => this.Parameter.Type;
        }

        /// <summary>
        /// A local variable reference.
        /// </summary>
        public record Local(Model.Local Definition) : Value
        {
            /// <inheritdoc/>
            public override Type Type => this.Definition.Type;
        }

        /// <summary>
        /// A local temporary reference.
        /// All instruction/computation results are this.
        /// </summary>
        public record Temp(Instruction.ValueProducer Instruction) : Value
        {
            /// <inheritdoc/>
            public override Type Type => this.Instruction.ResultType;
        }
    }
}
