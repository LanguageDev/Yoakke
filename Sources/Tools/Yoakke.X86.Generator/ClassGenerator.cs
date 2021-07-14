// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.X86.Generator.Model;

namespace Yoakke.X86.Generator
{
    /// <summary>
    /// The generator that generates x86 instruction classes.
    /// </summary>
    public class ClassGenerator
    {
        public StringBuilder Result { get; } = new();

        /// <summary>
        /// Generates code for a single <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        public void GenerateInstruction(Instruction instruction)
        {
            // First we check if we are qualified to deal with instruction
            if (instruction.Forms.All(form => form.Encodings.All(encoding => encoding.HasUnsupportedElement)))
            {
                // We can't really deal with this instruction, no encodings we know fully
                Console.WriteLine($"TODO: Missing things for all encodings for {instruction.Name}");
                return;
            }

            if (instruction.Forms.All(form => form.Operands.Any(op => !IsSupportedOperand(op))))
            {
                // We can't deal with this instruction because all forms have an operand we can't support
                Console.WriteLine($"TODO: Missing operand types for all forms {instruction.Name}");
                return;
            }

            if (instruction.Forms.All(form => form.Operands.Count > 2))
            {
                // We can't really deal with this instruction, too many operands
                Console.WriteLine($"TODO: Invalid operand count for {instruction.Name}");
                return;
            }

            // Doc comment
            this.Result
                .AppendLine("/// <summary>")
                .AppendLine($"/// {instruction.Summary}.")
                .AppendLine("/// </summary>");

            // Class
            var className = Capitalize(instruction.Name);
            this.Result
                .AppendLine($"public class {className}")
                .AppendLine("{");

            // Generate operand properties
            this.GenerateOperandProperties(instruction);

            this.Result.AppendLine("}").AppendLine();
        }

        private void GenerateOperandProperties(Instruction instruction)
        {
            // Checks if all forms have an operand count between certain numbers
            bool AllInBetween(int min, int max) =>
                instruction.Forms.All(form => form.Operands.Count >= min && form.Operands.Count <= max);

            if (AllInBetween(0, 0))
            {
                // Nullary operation
            }
            else if (AllInBetween(1, 1))
            {
                // Unary operation
            }
            else if (AllInBetween(2, 2))
            {
                // Binary operation
            }
            else if (AllInBetween(0, 1))
            {
                // Unary operation with optional operand
            }
            else if (AllInBetween(1, 2))
            {
                // Binary operation with one optional operand
            }
            else
            {
                Console.WriteLine($"Can't determine arity for {instruction.Name}");
            }
        }

        private static bool IsSupportedOperand(Operand operand) => SupportedOperandTypes.Contains(operand.Type);

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";

        private static readonly string[] SupportedOperandTypes =
        {
            "1", "3", "imm4", "imm8", "imm16", "imm32", "imm64",
            "al", "cl", "ax", "eax", "rax",
            "r8", "r16", "r32", "r64",
            "m",
            "m8", "m16", "m32", "m64", "m128", "m256", "m512",
        };
    }
}
