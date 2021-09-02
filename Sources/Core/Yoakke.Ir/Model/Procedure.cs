// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// A procedure in an assembly, consisting of basic blocks.
    /// </summary>
    public class Procedure : IAttributeTarget
    {
        /// <summary>
        /// The name of the procedure.
        /// </summary>
        public string Name { get; } = string.Empty;

        /// <summary>
        /// The basic block that first starts executing.
        /// </summary>
        public BasicBlock Entry { get; init; } = BasicBlock.Invalid;

        /// <summary>
        /// The basic blocks the procedure consists of.
        /// </summary>
        public IList<BasicBlock> BasicBlocks { get; init; } = new List<BasicBlock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Procedure"/> class.
        /// </summary>
        /// <param name="name">The name of the procedure.</param>
        public Procedure(string name)
        {
            this.Name = name;
        }

        #region AttributeTarget

        /// <inheritdoc/>
        public Attributes.AttributeTargets Flag => this.attributeTarget.Flag;

        private readonly AttributeTarget attributeTarget = new(Attributes.AttributeTargets.Procedure);

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
    }
}
