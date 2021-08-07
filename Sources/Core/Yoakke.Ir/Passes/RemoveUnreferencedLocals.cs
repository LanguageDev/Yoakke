// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Passes
{
    /// <summary>
    /// Removes any <see cref="Local"/> that is not referenced in a <see cref="IProcedure"/>.
    /// </summary>
    public class RemoveUnreferencedLocals : CodePassBase
    {
        /// <inheritdoc/>
        public override bool IsProcedure => true;

        /// <inheritdoc/>
        public override void Pass(IProcedure procedure)
        {
            // Concatenate all instructions
            var instructions = procedure.BasicBlocks.SelectMany(bb => bb.Instructions);
            // Select all local references from them
            var localsReferences = instructions
                .SelectMany(i => i.Operands)
                .OfType<Value.Local>()
                .Select(v => v.Definition)
                .ToHashSet();
            // Weed out unused
            for (var i = 0; i < procedure.Locals.Count; )
            {
                var local = procedure.Locals[i];
                if (localsReferences.Contains(local)) ++i;
                else procedure.Locals.RemoveAt(i);
            }
        }
    }
}
