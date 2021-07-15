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
                    result.AppendLine($"        if ({isValid}) return;");
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
            "al" or "cl" or "ax" or "eax" or "rax" => GenerateExactRegisterMatcher(index, operand.Type),
            "r8" or "r16" or "r32" or "r64" => GenerateSizedRegisterMatcher(index, int.Parse(operand.Type.Substring(1)) / 8),
            "m" => GenerateAddressMatcher(index),
            "m8" or "m16" or "m32" or "m64" or "m128" or "m256" or "m512" => GenerateIndirectMatcher(index, int.Parse(operand.Type.Substring(1)) / 8),
            "imm8" or "imm16" or "imm32" or "imm64" => GenerateConstantMatcher(index, int.Parse(operand.Type.Substring(3)) / 8),
            _ => throw new NotSupportedException(),
        };

        private static string GenerateConstantValueMatcher(int index, string value) =>
            $"this.Operands[{index}] is Constant c{index} && c{index} == {value}";

        private static string GenerateConstantMatcher(int index, int width) =>
            $"this.Operands[{index}] is Constant i{index} && (i{index}.Width is null || i{index}.Width == {ToDataWidth(width)})";

        private static string GenerateExactRegisterMatcher(int index, string registerName) =>
            $"ReferenceEquals(this.Operands[{index}], Registers.{Capitalize(registerName)})";

        private static string GenerateSizedRegisterMatcher(int index, int width) =>
            $"this.Operands[{index}] is Register r{index} && r{index}.Width == {ToDataWidth(width)}";

        private static string GenerateAddressMatcher(int index) =>
            $"this.Operands[{index}] is Address";

        private static string GenerateIndirectMatcher(int index, int width) =>
            $"this.Operands[{index}] is Indirect i{index} && i{index}.Width == {ToDataWidth(width)}";

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";

        private static string ToDataWidth(int width) => width switch
        {
            1 => "DataWidth.Byte",
            2 => "DataWidth.Word",
            4 => "DataWidth.Dword",
            8 => "DataWidth.Qword",
            _ => throw new NotSupportedException(),
        };
    }
}
