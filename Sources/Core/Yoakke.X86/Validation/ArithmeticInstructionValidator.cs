// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Validation
{
    /// <summary>
    /// A validator that validates all <see cref="Instructions.ArithmeticInstruction"/>s.
    /// This is used for ADD, SUB, ...
    /// </summary>
    public class ArithmeticInstructionValidator : IInstructionValidator
    {
        /// <inheritdoc/>
        public void Validate(AssemblyContext context, IInstruction instruction)
        {
            if (instruction.Operands.Count != 2) throw new ArgumentException("instruction requires 2 operands", nameof(instruction));
            var destination = instruction.Operands[0];
            var source = instruction.Operands[1];
            if (source is FarAddress || destination is FarAddress) throw new InvalidOperationException("operands can not be farjump addresses");
            var destSize = destination.GetSize(context);
            var srcSize = source.GetSize(context);
            if (destSize != srcSize) throw new InvalidOperationException("operand size mismatch");
            if (destination is Constant) throw new InvalidOperationException("destination operand cannot be a constant");
            if (destination.IsMemory && source.IsMemory) throw new InvalidOperationException("instruction must include at least one non-memory operand");
        }
    }
}
