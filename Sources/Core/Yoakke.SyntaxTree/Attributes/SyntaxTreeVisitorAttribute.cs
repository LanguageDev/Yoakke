// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SyntaxTree.Attributes
{
    /// <summary>
    /// An attribute to denote the generation of a visitor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class SyntaxTreeVisitorAttribute : Attribute
    {
        /// <summary>
        /// The name of the visitor to generate.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type to return from the visitor calls.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeVisitorAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the visitor to generate.</param>
        /// <param name="type">The type to return from the visitor calls.</param>
        public SyntaxTreeVisitorAttribute(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeVisitorAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the visitor to generate.</param>
        public SyntaxTreeVisitorAttribute(string name)
            : this(name, typeof(void))
        {
        }
    }
}
