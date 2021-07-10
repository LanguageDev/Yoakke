// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Validation
{
    /// <summary>
    /// A validator that only validates that the singular operand is not a far-jump address.
    /// </summary>
    public class OneOperandInstructionValidator : IInstructionValidator
    {
        public void Validate(AssemblyContext context, IInstruction instruction)
        {
            var one = (Instructions.OneOperandInstruction)instruction;
            if (one.Operand is FarAddress) throw new InvalidOperationException("operand can not be a far-jump target");
        }
    }
}
