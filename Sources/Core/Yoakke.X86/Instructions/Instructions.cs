// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Instructions
{
    public class Add : ArithmeticInstruction
    {
        public Add(IOperand dest, IOperand src)
            : base(Opcode.Add, dest, src)
        {
        }
    }
}
