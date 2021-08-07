// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

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
        /// A simple nothing value.
        /// </summary>
        public record Void : Value
        {
            /// <summary>
            /// A default instance to use.
            /// </summary>
            public static readonly Value Instance = new Void();

            /// <inheritdoc/>
            public override Type Type => Type.Void.Instance;
        }

        /// <summary>
        /// A procedure reference.
        /// </summary>
        public record Proc(IReadOnlyProcedure Procedure) : Value
        {
            /// <inheritdoc/>
            public override Type Type => new Type.Proc(
                this.Procedure.Return,
                this.Procedure.Parameters.Select(p => p.Type).ToList().AsValue());
        }

        /// <summary>
        /// A basic block reference.
        /// </summary>
        public record BasicBlock(IReadOnlyBasicBlock Block) : Value
        {
            /// <inheritdoc/>
            public override Type Type => new Type.Ptr(Type.Void.Instance);
        }

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

            /// <inheritdoc/>
            public virtual bool Equals(Temp? other) => ReferenceEquals(this.Instruction, other?.Instruction);

            /// <inheritdoc/>
            public override int GetHashCode() => RuntimeHelpers.GetHashCode(this.Instruction);
        }

        /// <summary>
        /// A signed or unsigned integer constant.
        /// </summary>
        public record Int : Value
        {
            /// <inheritdoc/>
            public override Type Type { get; }

            /// <summary>
            /// The value of this <see cref="Int"/>.
            /// </summary>
            public BigInt Value { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Int"/> class.
            /// </summary>
            /// <param name="signed">True, if the value should be signed.</param>
            /// <param name="value">The <see cref="BigInt"/> representing the value.</param>
            public Int(bool signed, BigInt value)
            {
                this.Type = new Type.Int(signed, value.Width);
                this.Value = value;
            }

            /// <summary>
            /// Deconstructs this <see cref="Int"/>.
            /// </summary>
            /// <param name="value">The <see cref="Value"/> gets written here.</param>
            public void Deconstruct(out BigInt value) => value = this.Value;
        }
    }
}
