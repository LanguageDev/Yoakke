// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// The base for all IR types.
    /// </summary>
    public abstract record Type : IInstructionArg
    {
        /// <summary>
        /// The nothing <see cref="Type"/>.
        /// </summary>
        public record Void : Type
        {
            /// <summary>
            /// A default <see cref="Void"/> instance.
            /// </summary>
            public static readonly Type Instance = new Void();
        }

        /// <summary>
        /// An integer <see cref="Type"/>.
        /// </summary>
        public record Int(bool Signed, int Bits) : Type;

        /// <summary>
        /// A procedure <see cref="Type"/>.
        /// </summary>
        public record Proc(Type Return, IReadOnlyValueList<Type> Parameters) : Type;

        /// <summary>
        /// A pointer <see cref="Type"/>.
        /// </summary>
        public record Ptr(Type Element) : Type;
    }
}
