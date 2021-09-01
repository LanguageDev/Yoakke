// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// Extensions for <see cref="IReadOnlyAttributeTarget"/>s and <see cref="IAttributeTarget"/>s.
    /// </summary>
    public static class AttributeTargetExtensions
    {
        /// <summary>
        /// Retrieves an <see cref="IAttribute"/> with a given name.
        /// </summary>
        /// <param name="target">The <see cref="IReadOnlyAttributeTarget"/> to get the attribute from.</param>
        /// <param name="name">The name of the <see cref="IAttribute"/> to query for.</param>
        /// <returns>The <see cref="IAttribute"/> with name <paramref name="name"/> attached to <paramref name="target"/>, or null,
        /// if no such <see cref="IAttribute"/> is found.</returns>
        public static IAttribute? GetAttribute(this IReadOnlyAttributeTarget target, string name) => target.TryGetAttribute(name, out var result)
            ? result
            : null;

        /// <summary>
        /// Gets an <see cref="IAttribute"/> of a given type.
        /// </summary>
        /// <typeparam name="TAttrib">The exact <see cref="IAttribute"/> type.</typeparam>
        /// <param name="target">The <see cref="IReadOnlyAttributeTarget"/> to get the attribute from.</param>
        /// <returns>The <see cref="IAttribute"/> of type <typeparamref name="TAttrib"/> attached to <paramref name="target"/>, or null, if no such
        /// <see cref="IAttribute"/> is found.</returns>
        public static TAttrib? GetAttribute<TAttrib>(this IReadOnlyAttributeTarget target)
            where TAttrib : IAttribute => target.TryGetAttribute<TAttrib>(out var result)
            ? result
            : default;
    }
}
