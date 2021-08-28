// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// An attribute target that can be queried for attributes, but can't attach any.
    /// </summary>
    public interface IReadOnlyAttributeTarget
    {
        /// <summary>
        /// The flag that annotates the kinds of attributes accepted.
        /// </summary>
        public AttributeTargets Flag { get; }

        /// <summary>
        /// Retrieves all <see cref="IAttribute"/>s from this target.
        /// </summary>
        /// <returns>All <see cref="IAttribute"/>s attached to this target.</returns>
        public IEnumerable<IAttribute> GetAttributes();

        /// <summary>
        /// Retrieves all <see cref="IAttribute"/>s from this target with a specified name.
        /// </summary>
        /// <param name="name">The name of the attributes to search for.</param>
        /// <returns>All <see cref="IAttribute"/>s attached to this target with name <paramref name="name"/>.</returns>
        public IEnumerable<IAttribute> GetAttributes(string name);

        /// <summary>
        /// Attempts to get an <see cref="IAttribute"/> with a specified name.
        /// </summary>
        /// <param name="name">The name of the attributes to search for.</param>
        /// <param name="attribute">The <see cref="IAttribute"/> gets written here, if found.</param>
        /// <returns>True, if the <see cref="IAttribute"/> with the name <paramref name="name"/> was found..</returns>
        public bool TryGetAttribute(string name, [MaybeNullWhen(false)] out IAttribute attribute);
    }
}
