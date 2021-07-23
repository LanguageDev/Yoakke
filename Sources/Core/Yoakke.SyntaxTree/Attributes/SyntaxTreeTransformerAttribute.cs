// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.SyntaxTree.Attributes
{
    /// <summary>
    /// An attribute to denote the generation of a transformer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class SyntaxTreeTransformerAttribute : Attribute
    {
        /// <summary>
        /// The name of the visitor to generate.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type the node gets transformed to. Can be null, if the new type is the same as the original.
        /// </summary>
        public Type? Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeTransformerAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the transformer to generate.</param>
        /// <param name="type">The type to return from the transformer calls.</param>
        public SyntaxTreeTransformerAttribute(string name, Type? type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeTransformerAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the transformer to generate.</param>
        public SyntaxTreeTransformerAttribute(string name)
            : this(name, null)
        {
        }
    }
}
