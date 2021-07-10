// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.X86.Validation;

namespace Yoakke.X86
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

        private readonly ValidatorCache validatorCache = new();

        public void Validate(AssemblyContext context)
        {
            foreach (var instruction in this.Elements.OfType<IInstruction>())
            {
                var validator = this.validatorCache.GetInstructionValidator(instruction);
                validator.Validate(context, instruction);
            }
        }
    }
}
