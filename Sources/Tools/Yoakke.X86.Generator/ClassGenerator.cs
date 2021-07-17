// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Generator.Model;

namespace Yoakke.X86.Generator
{
    /// <summary>
    /// The generator that generates x86 instruction classes.
    /// </summary>
    public static class ClassGenerator
    {
        /// <summary>
        /// Generates code for a single <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        /// <returns>The built class code for <paramref name="instruction"/>.</returns>
        public static string GenerateInstructionClass(Instruction instruction)
        {
            var result = new StringBuilder();

            // Class doc comment
            result
                .AppendLine("/// <summary>")
                .AppendLine($"/// {instruction.Summary}.")
                .AppendLine("/// </summary>");

            // Class start
            var className = Capitalize(instruction.Name);
            result
                .AppendLine($"public class {className}")
                .AppendLine("{");

            // For now we store all instruction operands in a read-only list
            result
                .AppendLine("    /// <summary>")
                .AppendLine($"    /// The operands of this instruction.")
                .AppendLine("    /// </summary>");
            result
                .AppendLine($"    public IReadOnlyList<IOperand> Operands {{ get; }}")
                .AppendLine();

            return result.ToString();
        }

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";
    }
}
