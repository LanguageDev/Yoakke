// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// A context for storing key information about the IR configuration.
    /// For example, this stores all attribute definitions.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The <see cref="IAttributeDefinition"/>s in this <see cref="Context"/>.
        /// </summary>
        public IReadOnlyDictionary<string, IAttributeDefinition> AttributeDefititions => this.attributeDefinitions;

        private readonly Dictionary<string, IAttributeDefinition> attributeDefinitions = new();

        /// <summary>
        /// Registers an <see cref="IAttributeDefinition"/> in this <see cref="Context"/>.
        /// </summary>
        /// <param name="definition">The <see cref="IAttributeDefinition"/> to register.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public Context AddAttributeDefinition(IAttributeDefinition definition)
        {
            this.attributeDefinitions.Add(definition.Name, definition);
            return this;
        }
    }
}
