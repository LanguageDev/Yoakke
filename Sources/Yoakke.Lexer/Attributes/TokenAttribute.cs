// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to define a token type in terms of a literal string to match.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TokenAttribute : Attribute
    {
        /// <summary>
        /// The text to match the token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAttribute"/> class.
        /// </summary>
        /// <param name="text">The text to match.</param>
        public TokenAttribute(string text)
        {
            this.Text = text;
        }
    }
}
