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
        private StringBuilder result { get; } = new();

        /// <summary>
        /// Generates code for a single <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        /// <returns>The built class.</returns>
        public string GenerateInstruction(Instruction instruction)
        {
            this.result.Clear();

            // Doc comment
            this.result
                .AppendLine("/// <summary>")
                .AppendLine($"/// {instruction.Summary}.")
                .AppendLine("/// </summary>");

            // Class
            var className = Capitalize(instruction.Name);
            this.result
                .AppendLine($"public class {className}")
                .AppendLine("{");

            // Generate operand properties
            this.GenerateOperandProperties(instruction);

            // We loop through each form with each encoding
            var anySupportedForms = false;
            foreach (var form in instruction.Forms)
            {
                if (form.Operands.Any(op => !IsSupportedOperand(op))) continue;

                var anySupportedEncodings = false;
                foreach (var encoding in form.Encodings)
                {
                    if (encoding.HasUnsupportedElement) continue;

                    // TODO: Process here

                    anySupportedEncodings = true;
                }

                if (!anySupportedEncodings) throw new NotSupportedException();
                anySupportedForms = true;
            }
            if (!anySupportedForms) throw new NotSupportedException();

            this.result.AppendLine("}");

            return this.result.ToString();
        }

        private void GenerateOperandProperties(Instruction instruction)
        {
            var operandNames = this.InferOperandNames(instruction);
            foreach (var name in operandNames)
            {
                var propertyName = Capitalize(name);

                // Property docs
                this.result
                    .AppendLine("    /// <summary>")
                    .AppendLine($"    /// The {name}.")
                    .AppendLine("    /// <*summary>");

                // The actual property
                this.result.AppendLine($"    public IOperand {propertyName} {{ get; }}");
            }
        }

        private IReadOnlyList<string> InferOperandNames(Instruction instruction)
        {
            // Checks if all forms have an operand count between certain numbers
            bool AllInBetween(int min, int max) =>
                instruction.Forms.All(form => form.Operands.Count >= min && form.Operands.Count <= max);

            // Checks if a predicate is true for all forms for a given operand
            bool OperandIs(int idx, Predicate<Operand> pred) =>
                instruction.Forms.All(form => pred(form.Operands[idx]));

            if (AllInBetween(0, 0))
            {
                // Nullary operation
                return Array.Empty<string>();
            }
            else if (AllInBetween(1, 1))
            {
                // Unary operation
                if (OperandIs(0, op => op.IsInput && op.IsOutput)) return new[] { "operand" };
                if (OperandIs(0, op => op.IsInput)) return new[] { "source" };
                if (OperandIs(0, op => op.IsOutput)) return new[] { "target" };
                return new[] { "operand" };
            }
            else if (AllInBetween(2, 2))
            {
                // Binary operation
                throw new NotSupportedException();
            }
            else if (AllInBetween(0, 1))
            {
                // Unary operation with optional operand
                throw new NotSupportedException();
            }
            else if (AllInBetween(1, 2))
            {
                // Binary operation with one optional operand
                throw new NotSupportedException();
            }
            else
            {
                throw new NotSupportedException();
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
