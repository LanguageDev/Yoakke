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
        /// The type of the node the visitor visits.
        /// </summary>
        public Type NodeType { get; set; }

        /// <summary>
        /// The type to return from the visitor calls.
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// The method name to use. The default is 'Visit'.
        /// </summary>
        public string? MethodName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeVisitorAttribute"/> class.
        /// </summary>
        /// <param name="nodeType">The type of the node the visitor visits.</param>
        /// <param name="returnType">The type to return from the visitor calls.</param>
        /// <param name="methodName">The method name to use.</param>
        public SyntaxTreeVisitorAttribute(Type nodeType, Type returnType, string? methodName = null)
        {
            this.NodeType = nodeType;
            this.ReturnType = returnType;
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeVisitorAttribute"/> class.
        /// </summary>
        /// <param name="nodeType">The type of the node the visitor visits.</param>
        public SyntaxTreeVisitorAttribute(Type nodeType)
            : this(nodeType, typeof(void))
        {
        }
    }
}
