// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// The collection of <see cref="IInstruction"/>s.
    /// </summary>
    public static class Instruction
    {
        /// <summary>
        /// Allocates a local variable with a given type.
        /// </summary>
        public record Local(Type Type) : IInstruction
        {
            /// <inheritdoc/>
            public bool IsBranch => false;
        }

        /// <summary>
        /// Returns from the current procedure.
        /// </summary>
        public record Ret(Value? Value) : IInstruction
        {
            /// <inheritdoc/>
            public bool IsBranch => true;
        }
    }
}
