// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Ir.Syntax;

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

        /// <summary>
        /// The <see cref="IInstructionSyntax"/>es in this <see cref="Context"/>.
        /// </summary>
        public IReadOnlyDictionary<string, IInstructionSyntax> InstructionSyntaxes => this.instructionSyntaxes;

        private readonly Dictionary<string, IAttributeDefinition> attributeDefinitions = new();
        private readonly Dictionary<string, IInstructionSyntax> instructionSyntaxes = new();

        /// <summary>
        /// Registers an <see cref="IAttributeDefinition"/> in this <see cref="Context"/>.
        /// </summary>
        /// <param name="definition">The <see cref="IAttributeDefinition"/> to register.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public Context WithAttributeDefinition(IAttributeDefinition definition)
        {
            this.attributeDefinitions.Add(definition.Name, definition);
            return this;
        }

        /// <summary>
        /// Registers an <see cref="IInstructionSyntax"/> in this <see cref="Context"/>.
        /// </summary>
        /// <param name="syntax">The <see cref="IInstructionSyntax"/> to register.</param>
        /// <returns>This instance, to be able to chain calls.</returns>
        public Context WithInstructionSyntax(IInstructionSyntax syntax)
        {
            this.instructionSyntaxes.Add(syntax.Name, syntax);
            return this;
        }
    }
}
