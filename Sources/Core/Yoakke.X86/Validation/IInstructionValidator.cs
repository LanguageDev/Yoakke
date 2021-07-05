// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Validation
{
    /// <summary>
    /// Interface for validating <see cref="IInstruction"/>s.
    /// </summary>
    public interface IInstructionValidator
    {
        /// <summary>
        /// Validates an <see cref="IInstruction"/> semantically.
        /// </summary>
        /// <param name="context">The <see cref="AssemblyContext"/> that contains extra information for validation.</param>
        /// <param name="instruction">The <see cref="IInstruction"/> to validate.</param>
        public void Validate(AssemblyContext context, IInstruction instruction);
    }
}
