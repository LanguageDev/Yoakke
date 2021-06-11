// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to annotate a rule over a transformation method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        /// <summary>
        /// The rule in grammar notation.
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// Initializes a new <see cref="RuleAttribute"/>.
        /// </summary>
        /// <param name="rule">The rule in grammar notation.</param>
        public RuleAttribute(string rule)
        {
            this.Rule = rule;
        }
    }
}
