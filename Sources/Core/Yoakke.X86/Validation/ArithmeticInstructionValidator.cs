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
        public void Validate(AssemblyContext context, IInstruction instruction)
        {
            var arith = (Instructions.ArithmeticInstruction)instruction;
            if (arith.Source is FarAddress || arith.Destination is FarAddress) throw new InvalidOperationException("operands can not be farjump addresses");
            var destSize = arith.Destination.GetSize(context);
            var srcSize = arith.Source.GetSize(context);
            if (destSize != srcSize) throw new InvalidOperationException("operand size mismatch");
            if (arith.Destination is Constant) throw new InvalidOperationException("destination operand cannot be a constant");
            if (arith.Destination.IsMemory && arith.Source.IsMemory) throw new InvalidOperationException("instruction must include at least one non-memory operand");
        }
    }
}
