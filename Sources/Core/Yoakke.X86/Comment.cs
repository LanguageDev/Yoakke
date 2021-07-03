// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86
{
    /// <summary>
    /// A comment in between assembly lines.
    /// </summary>
    public readonly struct Comment : ICodeElement
    {
        /// <summary>
        /// The text of the <see cref="Comment"/>.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> struct.
        /// </summary>
        /// <param name="text">The text of the comment.</param>
        public Comment(string text)
        {
            this.Text = text;
        }
    }
}
