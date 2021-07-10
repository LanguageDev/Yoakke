// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Lsp.Model
{
    /// <summary>
    /// An attribute to annotate that an element is only available since a given LSP version.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public class SinceAttribute : Attribute
    {
        /// <summary>
        /// The major version number.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// The minor version number.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// The patch number.
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SinceAttribute"/> class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The nimor version number.</param>
        /// <param name="patch">The patch number.</param>
        public SinceAttribute(int major = 1, int minor = 0, int patch = 0)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }
    }
}
