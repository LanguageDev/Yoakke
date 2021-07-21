// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SyntaxTree.Attributes
{
    /// <summary>
    /// An attribute to annotate if equality should be auto-implemented for an AST node.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementEqualityAttribute : Attribute
    {
        /// <summary>
        /// True, if should auto-implement, false if should not.
        /// </summary>
        public bool Implement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementEqualityAttribute"/> class.
        /// </summary>
        /// <param name="implement">True, if equality should be auto-implemented.</param>
        public ImplementEqualityAttribute(bool implement = true)
        {
            this.Implement = implement;
        }
    }
}
