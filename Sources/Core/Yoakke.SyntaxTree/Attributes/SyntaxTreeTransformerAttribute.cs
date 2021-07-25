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
        /// The name of the transformer to generate.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The type the node gets transformed to. Can be null, if the new type is the same as the original.
        /// </summary>
        public Type? TargetType { get; set; }

        /// <summary>
        /// The name of the custom transformer to use. Can be null, if no custom transformer is needed.
        /// </summary>
        public string? CustomTransformer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeTransformerAttribute"/> class.
        /// </summary>
        /// <param name="className">The name of the transformer to generate.</param>
        /// <param name="targetType">The type to return from the transformer calls.</param>
        public SyntaxTreeTransformerAttribute(string className, Type? targetType)
        {
            this.ClassName = className;
            this.TargetType = targetType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeTransformerAttribute"/> class.
        /// </summary>
        /// <param name="className">The name of the transformer to generate.</param>
        public SyntaxTreeTransformerAttribute(string className)
            : this(className, null)
        {
        }
    }
}
