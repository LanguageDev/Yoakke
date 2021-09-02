// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// Represents an attribute definition that can be put onto different assembly elements.
    ///
    /// An attribute in general has arguments and an assigned value, to have a nicer syntax.
    /// In general: [attribute_name(arg1, arg2, ...) = assigned_value].
    ///
    /// The above form is equivalent to [attribute_name(arg1, arg2, ..., assigned_value)].
    /// </summary>
    public interface IAttributeDefinition
    {
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// True, if multiple can be attached to the same element.
        /// </summary>
        public bool AllowMultiple { get; }

        /// <summary>
        /// The targets that the attribute can be applied to.
        /// </summary>
        public AttributeTargets Targets { get; }

        /// <summary>
        /// The parameter types that the attribute accepts.
        /// </summary>
        public IReadOnlyList<Type> ParameterTypes { get; }

        /// <summary>
        /// Instantiates an <see cref="IAttribute"/> from this <see cref="IAttributeDefinition"/>.
        /// </summary>
        /// <param name="arguments">The arguments passed in for the attribute.</param>
        /// <returns>The instantiated <see cref="IAttribute"/>.</returns>
        public IAttribute Instantiate(IReadOnlyList<Constant> arguments);
    }
}
