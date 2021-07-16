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
        /// Generates the code for the <see cref="Instruction"/> parser.
        /// </summary>
        /// <param name="instructionSet">The ISA to generate the parser for.</param>
        /// <param name="withClasses">The <see cref="Instruction"/>s that have a class generated.
        /// The ones that don't, won't generate parser code.</param>
        /// <returns>The code for the generated parser.</returns>
        public static string GenerateInstructionParser(InstructionSet instructionSet, ISet<Instruction> withClasses)
        {
            // Build the tree for the parser
            var root = new ParseNode();
            foreach (var instruction in instructionSet.Instructions)
            {
                // We skip it if it doesn't have a generated class
                if (!withClasses.Contains(instruction)) continue;

                foreach (var form in instruction.Forms)
                {
                    try
                    {
                        // We go through each operand and generate a matcher just to weed out unsupported stuff
                        foreach (var operand in form.Operands)
                        {
                            GenerateOperandMatcher(0, operand);
                        }

                        foreach (var encoding in form.Encodings)
                        {
                            // For safety we don't include anything unsupported
                            if (encoding.HasUnsupportedElement) continue;

                            // Add the element to the tree
                            root.AddEncoding(encoding);
                        }
                    }
                    catch (NotSupportedException)
                    {
                    }
                }
            }

            // Generate the parser function itself
            var result = new StringBuilder();

            // A function that walks through a single node
            void GenerateParserCase(int depth, ParseNode node)
            {
                if (node.Subnodes.Count > 0)
                {
                    // First we need to read in a byte to switch on that
                    result!
                        .Append(' ', depth * 4)
                        .AppendLine($"var byte{depth} = this.Peek({depth});");

                    // Now we switch on the alternatives
                    result
                        .Append(' ', depth * 4)
                        .AppendLine($"switch (byte{depth})")
                        .Append(' ', depth * 4)
                        .AppendLine("{");

                    // Generate cases
                    foreach (var (nextByte, subnode) in node.Subnodes)
                    {
                        result
                            .Append(' ', depth * 4)
                            .AppendLine($"case 0x{nextByte:x2}:")
                            .Append(' ', depth * 4)
                            .AppendLine("{");

                        GenerateParserCase(depth + 1, subnode);

                        result
                            .Append(' ', (depth + 1) * 4)
                            .AppendLine("break;")
                            .Append(' ', depth * 4)
                            .AppendLine("}");
                    }

                    // Close switch
                    result
                        .Append(' ', depth * 4)
                        .AppendLine("}");
                }

                foreach (var encoding in node.Encodings)
                {
                    GenerateParserLeafCase(depth, encoding);
                }
            }

            // A function that generates the leaf for a case
            void GenerateParserLeafCase(int depth, Model.Encoding encoding)
            {
                var operands = encoding.Form.Operands;
                var args = new string[encoding.Form.Operands.Count];

                // Prefix
                if (encoding.Prefixes.Count > 0)
                {
                    result
                        .Append(' ', depth * 4)
                        .AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (PREFIX)");
                    return;
                }

                /* Opcodes are already eaten */

                // ModRM
                if (encoding.ModRM is not null)
                {
                    result
                        .Append(' ', depth * 4)
                        .AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (ModRM)");
                    return;
                }

                // Postbyte
                if (encoding.Postbyte is not null)
                {
                    result
                        .Append(' ', depth * 4)
                        .AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (POSTBYTE)");
                    return;
                }

                // Immediates
                if (encoding.Immediates.Count > 0)
                {
                    result
                        .Append(' ', depth * 4)
                        .AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (IMMEDIATES)");
                    return;
                }

                // If it's a last 3 bit encoding, do that here
                for (var i = 0; i < encoding.Opcodes.Count; ++i)
                {
                    var last3 = encoding.Opcodes[i].Last3BitsEncodedOperand;
                    if (last3 is not null)
                    {
                        // TODO: Pass in size
                        args[last3.Value] = $"Registers.FromIndex(byte{i} & 0b111)";
                    }
                }

                // Deduce constant operands
                for (var i = 0; i < operands.Count; ++i)
                {
                    // We don't care about already deduced args
                    if (args[i] is not null) continue;

                    var op = GenerateParserConstantOperand(operands[i]);
                    if (op is not null) args[i] = op;
                }

                // We actually support everything
                var argsStr = string.Join(", ", args);
                var name = Capitalize(encoding.Form.Instruction.Name);
                result
                    .Append(' ', depth * 4)
                    .AppendLine($"return new {name}({argsStr});");
            }

            // Call for the root
            GenerateParserCase(0, root);

            return result.ToString();
        }

        /// <summary>
        /// Generates code for a single <see cref="Instruction"/> cass.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        /// <returns>The built class.</returns>
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

        // Returns a not null string for operands that are constants and don't require byte-parsing
        private static string? GenerateParserConstantOperand(Operand operand) => operand.Type switch
        {
            "1" or "3" => $"new Constant({operand.Type})",
            "al" => $"Registers.Al",
            "cl" => $"Registers.Cl",
            "ax" => $"Registers.Ax",
            "eax" => $"Registers.Eax",
            "rax" => $"Registers.Rax",
            _ => null,
        };
    }
}
