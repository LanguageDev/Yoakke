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
        private readonly Dictionary<string, IAttributeDefinition> attributeDefinitions = new();
        private readonly Dictionary<string, IInstructionSyntax> instructionSyntaxesByName = new();
        private readonly Dictionary<System.Type, IInstructionSyntax> instructionSyntaxesByType = new();

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
            this.instructionSyntaxesByName.Add(syntax.Name, syntax);
            this.instructionSyntaxesByType.Add(syntax.Type, syntax);
            return this;
        }

        /// <summary>
        /// Retrieves an <see cref="IAttributeDefinition"/> for a given name.
        /// </summary>
        /// <param name="name">The name of the <see cref="IAttributeDefinition"/>.</param>
        /// <returns>The found <see cref="IAttributeDefinition"/>.</returns>
        public IAttributeDefinition GetAttributeDefinition(string name) => this.attributeDefinitions[name];

        /// <summary>
        /// Retrieves an <see cref="IInstructionSyntax"/> for a given instruction name.
        /// </summary>
        /// <param name="name">The name of the instruction to retrieve syntax for.</param>
        /// <returns>The found <see cref="IInstructionSyntax"/>.</returns>
        public IInstructionSyntax GetInstructionSyntax(string name) => this.instructionSyntaxesByName[name];

        /// <summary>
        /// Retrieves an <see cref="IInstructionSyntax"/> for a given instruction implementation type.
        /// </summary>
        /// <param name="type">The type of the instruction to retrieve syntax for.</param>
        /// <returns>The found <see cref="IInstructionSyntax"/>.</returns>
        public IInstructionSyntax GetInstructionSyntax(System.Type type) => this.instructionSyntaxesByType[type];
    }
}
