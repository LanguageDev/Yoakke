// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Lexer.Attributes;

/// <summary>
/// An attribute to define a token type in terms of a regular expression.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class RegexAttribute : Attribute
{
    /// <summary>
    /// The regex to match the token.
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexAttribute"/> class.
    /// </summary>
    /// <param name="regex">The regular expression.</param>
    public RegexAttribute(string regex)
    {
        this.Regex = regex;
    }
}
