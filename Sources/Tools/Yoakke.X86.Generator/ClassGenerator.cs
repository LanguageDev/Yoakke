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
        /// <summary>
        /// Generates code for a single <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        /// <returns>The built class.</returns>
        public static string GenerateInstruction(Instruction instruction)
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

            // Generate the validator that checks if the operands are valid
            result
                .AppendLine("    public void Validate()")
                .AppendLine("    {");
            var validForms = 0;
            foreach (var form in instruction.Forms)
            {
                try
                {
                    var isValid = GenerateInstructionFormMatcher(form);
                    result.AppendLine($"        if({isValid}) return;");
                    ++validForms;
                }
                catch (NotSupportedException)
                { }
            }
            if (validForms == 0) throw new NotSupportedException();
            result
                .AppendLine($"        throw new InvalidOperationException(\"invalid operands for instruction {instruction.Name}\");")
                .AppendLine("    }");

            return result.ToString();
        }

        // Returns a condition that evaluates to true, if the form matches
        private static string GenerateInstructionFormMatcher(InstructionForm instructionForm)
        {
            var result = new StringBuilder($"this.Operands.Count == {instructionForm.Operands.Count}");
            for (var i = 0; i < instructionForm.Operands.Count; ++i)
            {
                var opValidator = GenerateOperandMatcher(i, instructionForm.Operands[i]);
                result.Append(" && ").Append(opValidator);
            }
            return result.ToString();
        }

        private static string GenerateOperandMatcher(int index, Operand operand) => operand.Type switch
        {
            "1" or "3" => GenerateConstantValueMatcher(index, operand.Type),
            _ => throw new NotSupportedException(),
        };

        private static string GenerateConstantValueMatcher(int index, string value) =>
            $"this.Operands[{index}] is Constant c{index} && c{index} == {value}";

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
