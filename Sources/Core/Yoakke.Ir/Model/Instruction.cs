// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// The core IR instructions.
    /// </summary>
    public abstract record Instruction : IAttributeTarget
    {
        #region AttributeTarget

        /// <inheritdoc/>
        public Attributes.AttributeTargets Flag => this.attributeTarget.Flag;

        private readonly AttributeTarget attributeTarget = new(Attributes.AttributeTargets.Instruction);

        /// <inheritdoc/>
        public IEnumerable<IAttribute> GetAttributes() => this.attributeTarget.GetAttributes();

        /// <inheritdoc/>
        public IEnumerable<IAttribute> GetAttributes(string name) => this.attributeTarget.GetAttributes(name);

        /// <inheritdoc/>
        public IEnumerable<TAttrib> GetAttributes<TAttrib>()
            where TAttrib : IAttribute => this.attributeTarget.GetAttributes<TAttrib>();

        /// <inheritdoc/>
        public bool TryGetAttribute(string name, [MaybeNullWhen(false)] out IAttribute attribute) =>
            this.attributeTarget.TryGetAttribute(name, out attribute);

        /// <inheritdoc/>
        public bool TryGetAttribute<TAttrib>([MaybeNullWhen(false)] out TAttrib attribute)
            where TAttrib : IAttribute => this.attributeTarget.TryGetAttribute(out attribute);

        /// <inheritdoc/>
        public void AddAttribute(IAttribute attribute) => this.attributeTarget.AddAttribute(attribute);

        #endregion AttributeTarget

        /// <summary>
        /// The result type of the produced value.
        /// Null, if the instruction produces no values.
        /// </summary>
        public virtual Type? ResultType => null;

        /* Variants */

        /// <summary>
        /// A no-operation instruction.
        /// </summary>
        public record Nop : Instruction;

        /// <summary>
        /// A return instruction.
        /// </summary>
        public record Ret(Value? Value = null) : Instruction;

        /// <summary>
        /// An addition instruction.
        /// </summary>
        public record Add(Value Left, Value Right) : Instruction
        {
            /// <inheritdoc/>
            // NOTE: Not necessarily a correct assumption
            public override Type? ResultType => this.Left.Type;
        }
    }
}
