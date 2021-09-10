// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// A simple <see cref="IAttributeTarget"/> implementation so we don't have to dump the same data structures
    /// and method implementations on all the different structures.
    /// </summary>
    internal class AttributeTarget : IAttributeTarget
    {
        /// <inheritdoc/>
        public AttributeTargets Flag { get; }

        private readonly Dictionary<string, List<IAttribute>> attributesByName = new();
        private readonly Dictionary<System.Type, List<IAttribute>> attributesByType = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeTarget"/> class.
        /// </summary>
        /// <param name="flag">The flag that identifies what kind of target this is backing.</param>
        public AttributeTarget(AttributeTargets flag)
        {
            this.Flag = flag;
        }

        /// <inheritdoc/>
        public IEnumerable<IAttribute> GetAttributes() => this.attributesByName.Values.SelectMany(x => x);

        /// <inheritdoc/>
        public IEnumerable<IAttribute> GetAttributes(string name) => this.attributesByName.TryGetValue(name, out var attrs)
            ? attrs
            : Enumerable.Empty<IAttribute>();

        /// <inheritdoc/>
        public IEnumerable<TAttrib> GetAttributes<TAttrib>()
            where TAttrib : IAttribute => this.attributesByType.TryGetValue(typeof(TAttrib), out var attrs)
            ? attrs.OfType<TAttrib>()
            : Enumerable.Empty<TAttrib>();

        /// <inheritdoc/>
        public bool TryGetAttribute(string name, [MaybeNullWhen(false)] out IAttribute attribute)
        {
            if (!this.attributesByName.TryGetValue(name, out var attrList))
            {
                attribute = null;
                return false;
            }
            if (attrList.Count == 0)
            {
                attribute = null;
                return false;
            }
            attribute = attrList[0];
            return true;
        }

        /// <inheritdoc/>
        public bool TryGetAttribute<TAttrib>([MaybeNullWhen(false)] out TAttrib attribute)
            where TAttrib : IAttribute
        {
            if (!this.attributesByType.TryGetValue(typeof(TAttrib), out var attrList))
            {
                attribute = default;
                return false;
            }
            if (attrList.Count == 0)
            {
                attribute = default;
                return false;
            }
            attribute = (TAttrib)attrList[0];
            return true;
        }

        /// <inheritdoc/>
        public void AddAttribute(IAttribute attribute)
        {
            if (!attribute.Definition.Targets.HasFlag(this.Flag))
            {
                throw new InvalidOperationException("The attribute is invalid for this target");
            }

            var name = attribute.Definition.Name;
            var type = attribute.GetType();
            if (!this.attributesByName.TryGetValue(name, out var byNameList))
            {
                byNameList = new();
                this.attributesByName.Add(name, byNameList);
            }
            if (!this.attributesByType.TryGetValue(type, out var byTypeList))
            {
                byTypeList = new();
                this.attributesByType.Add(type, byTypeList);
            }

            if (!attribute.Definition.AllowMultiple && byNameList.Count > 0)
            {
                throw new InvalidOperationException("The attribute does not allow for multiple additions to the same target");
            }

            byNameList.Add(attribute);
            byTypeList.Add(attribute);
        }
    }
}
