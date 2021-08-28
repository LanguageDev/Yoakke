// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Ir.Model.Types;
using Yoakke.Ir.Model.Values;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// Represents an attribute definition that can be put onto different assembly elements.
    ///
    /// An attribute in general has arguments and an assigned value, to have a nicer syntax.
    /// In general: [attribute_name(arg1, arg2, ...) = assigned_value].
    ///
    /// The arguments must be constants, while the assigned value has to come from a pre-defined set of values
    /// defined by the attribute itself.
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
        public IReadOnlyList<IType> ParameterTypes { get; }

        /// <summary>
        /// The value set that can be assigned.
        /// </summary>
        public IReadOnlyCollection<string> AssignableValues { get; }

        /// <summary>
        /// Instantiates an <see cref="IAttribute"/> from this <see cref="IAttributeDefinition"/>.
        /// </summary>
        /// <param name="arguments">The arguments to pass.</param>
        /// <param name="assignedValue">The assigned value to pass.</param>
        /// <returns>The instantiated <see cref="IAttribute"/>.</returns>
        public IAttribute Instantiate(IReadOnlyList<IConstant> arguments, string? assignedValue);
    }
}
