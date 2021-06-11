// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum to generate a lexer for it's token types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class LexerAttribute : Attribute
    {
        /// <summary>
        /// The lexer classes name to generate.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerAttribute"/> class.
        /// </summary>
        /// <param name="className">The lexer classes name to generate.</param>
        public LexerAttribute(string className)
        {
            this.ClassName = className;
        }
    }
}
