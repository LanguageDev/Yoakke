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
    /// Generates validation code for x86 instructions.
    /// </summary>
    public static class ValidatorGenerator
    {
        /// <summary>
        /// Generates validation code for a single <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate validation code for.</param>
        /// <returns>A block of code that assumes that the <paramref name="instruction"/> is in a variable called
        /// 'instruction', and will return true, if it's in a valid form, false otherwise.</returns>
        public static string GenerateInstructionValidator(Instruction instruction)
        {
            var result = new StringBuilder();

            var validForms = 0;
            foreach (var form in instruction.Forms)
            {
                try
                {
                    var isValid = GenerateInstructionFormMatcher(form);
                    result.AppendLine($"if ({isValid}) return true;");
                    ++validForms;
                }
                catch (NotSupportedException)
                { }
            }
            if (validForms == 0) throw new NotSupportedException();

            result.AppendLine("return false;");

            return result.ToString();
        }

        // Returns a condition that evaluates to true, if the form matches
        private static string GenerateInstructionFormMatcher(InstructionForm instructionForm)
        {
            var result = new StringBuilder($"instruction.Operands.Count == {instructionForm.Operands.Count}");
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
            "imm8" or "imm16" or "imm32" or "imm64" or "rel8" or "rel32" => GenerateConstantMatcher(index, int.Parse(operand.Type.Substring(3)) / 8),
            _ => throw new NotSupportedException(),
        };

        private static string GenerateConstantValueMatcher(int index, string value) =>
            $"instruction.Operands[{index}] is Constant c{index} && c{index} == {value}";

        private static string GenerateConstantMatcher(int index, int width) =>
            $"instruction.Operands[{index}] is Constant i{index} && (i{index}.Width is null || i{index}.Width == {ToDataWidth(width)})";

        private static string GenerateExactRegisterMatcher(int index, string registerName) =>
            $"ReferenceEquals(instruction.Operands[{index}], Registers.{Capitalize(registerName)})";

        private static string GenerateSizedRegisterMatcher(int index, int width) =>
            $"instruction.Operands[{index}] is Register r{index} && r{index}.Width == {ToDataWidth(width)}";

        private static string GenerateAddressMatcher(int index) =>
            $"instruction.Operands[{index}] is Address";

        private static string GenerateIndirectMatcher(int index, int width) =>
            $"instruction.Operands[{index}] is Indirect i{index} && i{index}.Width == {ToDataWidth(width)}";

        private static string ToDataWidth(int width) => width switch
        {
            1 => "DataWidth.Byte",
            2 => "DataWidth.Word",
            4 => "DataWidth.Dword",
            8 => "DataWidth.Qword",
            _ => throw new NotSupportedException(),
        };

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";
    }
}
