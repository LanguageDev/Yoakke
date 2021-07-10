// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.X86.Validation
{
    /// <summary>
    /// An <see cref="IInstructionValidator"/> that does nothing.
    /// </summary>
    public class NullInstructionValidator : IInstructionValidator
    {
        /// <summary>
        /// A default instance of this <see cref="NullInstructionValidator"/>.
        /// </summary>
        public static readonly NullInstructionValidator Default = new();

        /// <inheritdoc/>
        public void Validate(AssemblyContext context, IInstruction instruction)
        {
        }
    }
}
