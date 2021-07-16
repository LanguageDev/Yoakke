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
    /// The generator that generates the x86 instruction parser.
    /// </summary>
    public class ParserGenerator
    {
        /// <summary>
        /// Generates the code for the <see cref="Instruction"/> parser.
        /// </summary>
        /// <param name="instructionSet">The ISA to generate the parser for.</param>
        /// <param name="withClasses">The <see cref="Instruction"/>s that have a class generated.
        /// The ones that don't, won't generate parser code.</param>
        /// <returns>The code for the generated parser.</returns>
        public static string Generate(InstructionSet instructionSet, ISet<Instruction> withClasses)
        {
            var generator = new ParserGenerator(instructionSet, withClasses);
            generator.BuildParserTree();
            generator.GenerateParser();
            return generator.resultBuilder.ToString();
        }

        private readonly InstructionSet instructionSet;
        private readonly ISet<Instruction> withClasses;
        private readonly StringBuilder resultBuilder;
        private ParseNode parseTree = new();
        private int indent = 0;

        private ParserGenerator(InstructionSet instructionSet, ISet<Instruction> withClasses)
        {
            this.instructionSet = instructionSet;
            this.withClasses = withClasses;
            this.resultBuilder = new();
        }

        private void GenerateParser() => this.GenerateNode(MatchType.Opcode, this.parseTree);

        private void GenerateNode(MatchType matchType, ParseNode node)
        {
            switch (matchType)
            {
            case MatchType.Opcode:
                this.GenerateOpcodeNode(node);
                break;
            case MatchType.ModRmReg:
                this.GenerateModRmNode(node);
                break;
            case MatchType.Prefix:
                this.GeneratePrefixNode(node);
                break;

            default: throw new NotImplementedException();
            }

            foreach (var encoding in node.Encodings) this.GenerateLeaf(encoding);
        }

        private void GenerateOpcodeNode(ParseNode node)
        {
            if (node.Subnodes.Count == 0) return;

            // Read in a byte
            this.Indented().AppendLine($"var byte{this.indent} = this.ParseByte();");
            // Switch on alternatives
            this.Indented().AppendLine($"switch (byte{this.indent})");
            this.Indented().AppendLine("{");

            // The different cases
            foreach (var (nextByte, subnode) in node.Subnodes)
            {
                this.Indented().AppendLine($"case 0x{nextByte:x2}:");
                this.Indented().AppendLine("{");
                this.Indent();

                this.GenerateNode(subnode.Item1, subnode.Item2);

                this.Indented().AppendLine("break;");
                this.Unindent();
                this.Indented().AppendLine("}");
            }

            this.Indented().AppendLine("}");
        }

        private void GenerateModRmNode(ParseNode node)
        {
            if (node.Subnodes.Count == 0) return;

            this.Indented().AppendLine("// TODO: MOD/RM");
        }

        private void GeneratePrefixNode(ParseNode node)
        {
            if (node.Subnodes.Count == 0) return;

            foreach (var (prefixByte, subnode) in node.Subnodes)
            {
                this.Indented().AppendLine($"if (this.HasPrefix(0x{prefixByte:x2})");
                this.Indented().AppendLine("{");
                this.Indent();

                this.GenerateNode(subnode.Item1, subnode.Item2);

                this.Unindent();
                this.Indented().AppendLine("}");
            }
        }

        private void GenerateLeaf(Model.Encoding encoding)
        {
            var operands = encoding.Form.Operands;
            var args = new string[encoding.Form.Operands.Count];

            /* Prefixes are already matched in the tree are already eaten */

            /* Opcodes are already eaten */

            // ModRM
            if (encoding.ModRM is not null)
            {
                var modrm = encoding.ModRM;
                if (!modrm.Reg.StartsWith('#'))
                {
                    // It's an opcode extension
                    this.Indented().AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (ModRM opcode extension)");
                    return;
                }
                else
                {
                    this.Indented().AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (ModRM operands)");
                    return;
                }
            }

            // Postbyte
            if (encoding.Postbyte is not null)
            {
                this.Indented().AppendLine($"// TODO: Missing encoding for {encoding.Form.Instruction.Name} (POSTBYTE)");
                return;
            }

            // Immediates
            for (var i = 0; i < encoding.Immediates.Count; ++i)
            {
                var immediate = encoding.Immediates[i];
                this.Indented().AppendLine($"var imm{i} = this.ParseImmediate({immediate.Size});");
                args[immediate.OperandNumber] = $"imm{i}";
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
            this.Indented().AppendLine($"return new {name}({argsStr});");
        }

        private void BuildParserTree()
        {
            // Build the tree for the parser
            var root = new ParseNode();
            foreach (var instruction in this.instructionSet.Instructions)
            {
                // We skip it if it doesn't have a generated class
                if (!this.withClasses.Contains(instruction)) continue;

                foreach (var form in instruction.Forms)
                {
                    // Skip forms that have an unsupported operand
                    if (!form.Operands.All(SupportsOperand)) continue;

                    foreach (var encoding in form.Encodings)
                    {
                        // For safety we don't include anything unsupported
                        if (encoding.HasUnsupportedElement) continue;

                        // Add the element to the tree
                        root.AddEncoding(encoding);
                    }
                }
            }

            // Store as the new tree
            this.parseTree = root;
        }

        private StringBuilder Indented() => this.resultBuilder.Append(' ', this.indent * 4);

        private void Indent() => ++this.indent;

        private void Unindent() => --this.indent;

        private static bool SupportsOperand(Operand operand) => operand.Type switch
        {
               "1" or "3"
            or "al" or "cl" or "ax" or "eax" or "rax"
            or "r8" or "r16" or "r32" or "r64"
            or "m"
            or "m8" or "m16" or "m32" or "m64" or "m128" or "m256" or "m512"
            or "imm8" or "imm16" or "imm32" or "imm64" => true,
            _ => false,
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

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";
    }
}
