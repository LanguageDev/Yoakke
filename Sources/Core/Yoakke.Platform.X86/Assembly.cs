// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.Platform.X86
{
    /// <summary>
    /// An x86 assembly.
    /// </summary>
    public class Assembly
    {
        // TODO: Sections?

        /// <summary>
        /// The <see cref="ICodeElement"/>s this <see cref="Assembly"/>s code consists of.
        /// </summary>
        public IReadOnlyList<ICodeElement> Elements { get; init; } = Array.Empty<ICodeElement>();

        /// <summary>
        /// Validates this <see cref="Assembly"/>.
        /// </summary>
        /// <param name="context">The <see cref="AssemblyContext"/> to use for validation.</param>
        public void Validate(AssemblyContext context)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
