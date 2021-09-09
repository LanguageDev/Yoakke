// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Ir.Model.Attributes
{
    /// <summary>
    /// Attribute target flags to annotate on which element an attribute can be applied.
    /// </summary>
    [Flags]
    public enum AttributeTargets
    {
        /// <summary>
        /// The attribute can be used on anything.
        /// </summary>
        All = 0b11111111,

        /// <summary>
        /// The attribute can be used on an assembly.
        /// </summary>
        Assembly = 1 << 0,

        /// <summary>
        /// The attribute can be used on a procedure.
        /// </summary>
        Procedure = 1 << 1,

        /// <summary>
        /// The attribute can be used on a basic block.
        /// </summary>
        BasicBlock = 1 << 2,

        /// <summary>
        /// The attribute can be used on an instruction.
        /// </summary>
        Instruction = 1 << 3,

        /// <summary>
        /// The attribute can be used on a type definition.
        /// </summary>
        TypeDefinition = 1 << 4,

        /// <summary>
        /// The attribute can be used on a field of a type.
        /// </summary>
        TypeField = 1 << 5,

        /// <summary>
        /// The attribute can be used on a parameter.
        /// </summary>
        Parameter = 1 << 6,

        /// <summary>
        /// The attribute can be used on a return value.
        /// </summary>
        ReturnValue = 1 << 7,
    }
}
