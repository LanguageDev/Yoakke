// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var code = generator.resultBuilder.ToString();
            code = RemoveCodeAfterReturn(code);
            code = RemovePrefixDuplicates(code);
            code = MergeEnclosedIf(code);
            code = MergeIdenticalCases(code);
            return code;
        }

        private static string RemoveCodeAfterReturn(string code)
        {
            var result = new StringBuilder();
            var lines = SplitIntoLines(code);
            var ignore = false;
            foreach (var line in lines)
            {
                var trimmedLine = line.TrimStart();

                if (trimmedLine.StartsWith("}")) ignore = false;
                if (ignore) continue;
                ignore = line.TrimStart().StartsWith("return");
                result.AppendLine(line);
            }
            return result.ToString();
        }

        private static string RemovePrefixDuplicates(string code)
        {
            var lines = SplitIntoLines(code);
            var result = new StringBuilder();
            for (var i = 0; i < lines.Count; )
            {
                if (lines[i].Contains("HasPrefix"))
                {
                    // We potentially can "optimize away" this line by merging it with the next lines
                    // Take the condition
                    var hasPrefixCheck = lines[i].Trim().Substring(4);
                    hasPrefixCheck = hasPrefixCheck.Substring(0, hasPrefixCheck.Length - 1);
                    // Jump over the if (HasPrefix(...))
                    var j = i + 1;
                    // Collect out the lines between the braces
                    var insideLines = ParseCurlyContents(lines, ref j);
                    // We pre-process the lines a bit to make matching easier
                    insideLines = insideLines.Select(l => l.Substring(4)).ToList();

                    // Now we have all the lines in between if (HasPrefix(...)) { HERE }
                    // Figure out if these next lines match
                    var replacementLines = new List<string>();
                    var canCutOut = true;
                    for (var k = 0; k < insideLines.Count; ++k)
                    {
                        var outsideLine = lines[j + k];
                        if (outsideLine != insideLines[k])
                        {
                            var replacement = FindReplacementCombo(hasPrefixCheck, insideLines[k], outsideLine);
                            if (replacement is null)
                            {
                                canCutOut = false;
                                break;
                            }
                            replacementLines.Add(replacement);
                        }
                        else
                        {
                            replacementLines.Add(outsideLine);
                        }
                    }
                    if (canCutOut)
                    {
                        // Found a match, write in replacement, advance
                        foreach (var line in replacementLines) result.AppendLine(line);
                        i = j + insideLines.Count;
                        continue;
                    }
                }
                result.AppendLine(lines[i]);
                ++i;
            }
            return result.ToString();
        }

        private static string MergeEnclosedIf(string code)
        {
            var lines = SplitIntoLines(code);
            var result = new StringBuilder();

            for (var i = 0; i < lines.Count; )
            {
                if (lines[i].Contains("if (") && lines[i + 2].Contains("if ("))
                {
                    var outerIf = lines[i];
                    var innerIf = lines[i + 2];
                    i += 3;
                    // Take what's in the inner if-statement
                    var innerIfContent = ParseCurlyContents(lines, ref i);
                    // Skip over the close brace of the outer if
                    ++i;
                    // Un-indent it once
                    innerIfContent = innerIfContent.Select(l => l.Substring(4)).ToList();
                    // Piece together the new if
                    var combinedIf = $"{outerIf.Substring(0, outerIf.Length - 1)} && {innerIf.TrimStart().Substring(4)}";
                    // Get the indentation
                    var indentation = new string(outerIf.TakeWhile(c => c == ' ').ToArray());
                    // Build it up
                    result.AppendLine(combinedIf);
                    result.AppendLine($"{indentation}{{");
                    foreach (var line in innerIfContent) result.AppendLine(line);
                    result.AppendLine($"{indentation}}}");
                }
                else
                {
                    result.AppendLine(lines[i]);
                    ++i;
                }
            }

            return result.ToString();
        }

        private static string MergeIdenticalCases(string code)
        {
            var lines = SplitIntoLines(code);
            var result = new StringBuilder();

            for (var i = 0; i < lines.Count;)
            {
                if (lines[i].Contains("case"))
                {
                    var j = i + 1;
                    var caseContents = ParseCurlyContents(lines, ref j);
                    var identicalCases = new List<string> { lines[i] };
                    while (lines[j].Contains("case"))
                    {
                        var caseLine = lines[j];
                        ++j;
                        var caseContents2 = ParseCurlyContents(lines, ref j);
                        if (!caseContents.SequenceEqual(caseContents2)) break;
                        identicalCases.Add(caseLine);
                    }

                    if (identicalCases.Count > 1)
                    {
                        var caseIndent = new string(lines[i].TakeWhile(c => c == ' ').ToArray());
                        foreach (var c in identicalCases) result.AppendLine(c);
                        result.AppendLine($"{caseIndent}{{");
                        foreach (var l in caseContents) result.AppendLine(l);
                        result.AppendLine($"{caseIndent}}}");
                        i += identicalCases.Count * (3 + caseContents.Count);
                        continue;
                    }
                }
                result.AppendLine(lines[i]);
                ++i;
            }

            return result.ToString();
        }

        private static string? FindReplacementCombo(string prefixCondition, string withPrefix, string withoutPrefix)
        {
            var dataWidths = new[]
            {
                ("DataWidth.Byte", "Registers.Al"),
                ("DataWidth.Word", "Registers.Ax"),
                ("DataWidth.Dword", "Registers.Eax"),
            };

            string MacroLine(string line, (string, string) args) => line
                .Replace(args.Item1, "$DW$")
                .Replace(args.Item2, "$REG$");

            foreach (var withPrefixWidth in dataWidths)
            {
                var withPrefixMacrod = MacroLine(withPrefix, withPrefixWidth);
                foreach (var withoutPrefixWidth in dataWidths)
                {
                    if (withPrefixWidth == withoutPrefixWidth) continue;

                    var withoutPrefixMacrod = MacroLine(withoutPrefix, withoutPrefixWidth);
                    if (withPrefixMacrod != withoutPrefixMacrod) continue;

                    // It's a match, construct the common line
                    return withPrefixMacrod
                        .Replace("$DW$", $"{prefixCondition} ? {withPrefixWidth.Item1} : {withoutPrefixWidth.Item1}")
                        .Replace("$REG$", $"{prefixCondition} ? {withPrefixWidth.Item2} : {withoutPrefixWidth.Item2}");
                }
            }

            return null;
        }

        private static List<string> ParseCurlyContents(List<string> lines, ref int j)
        {
            var insideLines = new List<string>();
            Debug.Assert(lines[j].EndsWith("{"), "This function should be called when the current line is a '{'");
            ++j;
            var depth = 1;
            while (true)
            {
                var l = lines[j];
                ++j;

                if (l.EndsWith("{")) ++depth;
                else if (l.EndsWith("}")) --depth;

                if (depth == 0) break;

                insideLines.Add(l);
            }
            return insideLines;
        }

        private static List<string> SplitIntoLines(string code)
        {
            var lines = new List<string>();
            var reader = new StringReader(code);
            while (true)
            {
                var line = reader.ReadLine();
                if (line is null) break;
                lines.Add(line);
            }
            return lines;
        }

        private readonly InstructionSet instructionSet;
        private readonly ISet<Instruction> withClasses;
        private readonly StringBuilder resultBuilder;
        private ParseNode parseTree = new(MatchType.None);
        private int indent = 0;
        private bool modrmParsed;

        private ParserGenerator(InstructionSet instructionSet, ISet<Instruction> withClasses)
        {
            this.instructionSet = instructionSet;
            this.withClasses = withClasses;
            this.resultBuilder = new();
        }

        private void GenerateParser() => this.GenerateNode(0, this.parseTree);

        private void GenerateNode(int byteDepth, ParseNode node)
        {
            var subnodesByType = node.Subnodes.GroupBy(s => s.Value.Type);

            var byOpcode = subnodesByType.FirstOrDefault(g => g.Key == MatchType.Opcode);
            var byModRmReg = subnodesByType.FirstOrDefault(g => g.Key == MatchType.ModRmReg);
            var byPrefix = subnodesByType.FirstOrDefault(g => g.Key == MatchType.Prefix);

            this.GenerateOpcodeMatches(byteDepth, byOpcode);
            this.GenerateModRmRegMatches(byteDepth, byModRmReg);
            this.GeneratePrefixMatches(byteDepth, byPrefix);

            foreach (var encoding in node.Encodings) this.GenerateLeaf(encoding);
        }

        private void GenerateOpcodeMatches(int byteDepth, IEnumerable<KeyValuePair<byte, ParseNode>>? nodes)
        {
            if (nodes is null) return;

            // Read in a byte
            this.Indented().AppendLine($"if (this.TryParseByte(out var byte{byteDepth}))");
            this.Indented().AppendLine("{");
            this.Indent();
            // Switch on alternatives
            this.Indented().AppendLine($"switch (byte{byteDepth})");
            this.Indented().AppendLine("{");

            // The different cases
            foreach (var (nextByte, subnode) in nodes)
            {
                this.Indented().AppendLine($"case 0x{nextByte:x2}:");
                this.Indented().AppendLine("{");
                this.Indent();

                this.GenerateNode(byteDepth + 1, subnode);

                this.Indented().AppendLine("break;");
                this.Unindent();
                this.Indented().AppendLine("}");
            }

            this.Indented().AppendLine("}");
            // We didn't use the byte, un-eat it
            this.Indented().AppendLine("this.UnparseByte();");
            this.Unindent();
            this.Indented().AppendLine("}");
        }

        private void GenerateModRmRegMatches(int byteDepth, IEnumerable<KeyValuePair<byte, ParseNode>>? nodes)
        {
            if (nodes is null) return;

            Debug.Assert(nodes.Count() <= 8, "There must be at most 8 encodings for ModRM extensions");

            // Read in the ModRM byte
            // NOTE: We tag modrm byte with indentation to avoid name collision
            this.modrmParsed = true;
            this.Indented().AppendLine($"if (this.TryParseByte(out modrm_byte))");
            this.Indented().AppendLine("{");
            this.Indent();
            // Switch on alternatives for the register
            this.Indented().AppendLine($"switch ((modrm_byte >> 3) & 0b111)");
            this.Indented().AppendLine("{");

            // The different cases
            foreach (var (regByte, subnode) in nodes)
            {
                this.Indented().AppendLine($"case 0x{regByte:x2}:");
                this.Indented().AppendLine("{");
                this.Indent();

                this.GenerateNode(byteDepth + 1, subnode);

                this.Indented().AppendLine("break;");
                this.Unindent();
                this.Indented().AppendLine("}");
            }

            this.Indented().AppendLine("}");
            // We didn't use the byte, un-eat it
            this.Indented().AppendLine("this.UnparseByte();");
            this.modrmParsed = false;
            this.Unindent();
            this.Indented().AppendLine("}");
        }

        private void GeneratePrefixMatches(int byteDepth, IEnumerable<KeyValuePair<byte, ParseNode>>? nodes)
        {
            if (nodes is null) return;

            foreach (var (prefixByte, subnode) in nodes)
            {
                this.Indented().AppendLine($"if (HasPrefix(0x{prefixByte:x2}))");
                this.Indented().AppendLine("{");
                this.Indent();

                this.GenerateNode(byteDepth, subnode);

                this.Unindent();
                this.Indented().AppendLine("}");
            }
        }

        private void GenerateLeaf(Model.Encoding encoding)
        {
            var operands = encoding.Form.Operands;
            var args = new string[encoding.Form.Operands.Count];
            var haveToCloseIf = false;

            /* Prefixes are already matched in the tree are already eaten */

            /* Opcodes are already eaten */

            // ModRM
            if (encoding.ModRM is not null)
            {
                var modrm = encoding.ModRM;
                // TODO: Check if we can safely skip this?
                // We handle this with the regular ModRM
                if (modrm.Mode == "11") return;

                haveToCloseIf = !this.modrmParsed;
                // Consume ModRM, if we haven't already
                if (!this.modrmParsed)
                {
                    // NOTE: We tag modrm byte with indentation to avoid name collision
                    this.Indented().AppendLine($"if (this.TryParseByte(out modrm_byte))");
                    this.Indented().AppendLine("{");
                    this.Indent();
                }

                if (modrm.Reg.StartsWith('#'))
                {
                    // Regular ModRM, not opcode extension
                    // We can just convert the reg
                    var regOperandIndex = int.Parse(modrm.Reg.Substring(1));
                    var size = GetDataWidthForOperand(encoding.Form.Operands[regOperandIndex]);
                    args[regOperandIndex] = $"FromRegisterIndex((modrm_byte >> 3) & 0b111, {size})";
                }

                // Mode and RM
                Debug.Assert(modrm.Mode == "11" || modrm.Mode == modrm.Rm, "Mode and RM have to reference the same argument");
                var rmOperandIndex = int.Parse(modrm.Rm.Substring(1));
                var rmOperandSize = GetDataWidthForOperand(encoding.Form.Operands[rmOperandIndex]);
                // NOTE: We tag RM with indentation to avoid name collision
                this.Indented().AppendLine($"op{rmOperandIndex} = this.ParseRM(modrm_byte, {rmOperandSize});");
                args[rmOperandIndex] = $"op{rmOperandIndex}";
                // NOTE: We don't un-parse here, we assume this has to work for now
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
                var operandIndex = immediate.OperandNumber;
                // NOTE: We tag immediates with indentation to avoid name collision
                this.Indented().AppendLine($"op{operandIndex} = this.ParseImmediate({GetDataWidthForSize(immediate.Size)});");
                args[operandIndex] = $"op{operandIndex}";
            }

            // Code offsets
            for (var i = 0; i < encoding.CodeOffsets.Count; ++i)
            {
                var immediate = encoding.CodeOffsets[i];
                var operandIndex = immediate.OperandNumber;
                // NOTE: We tag immediates with indentation to avoid name collision
                this.Indented().AppendLine($"op{operandIndex} = this.ParseCodeOffset({GetDataWidthForSize(immediate.Size)});");
                args[operandIndex] = $"op{operandIndex}";
            }

            // If it's a last 3 bit encoding, do that here
            for (var i = 0; i < encoding.Opcodes.Count; ++i)
            {
                var last3 = encoding.Opcodes[i].Last3BitsEncodedOperand;
                if (last3 is not null)
                {
                    var size = GetDataWidthForOperand(encoding.Form.Operands[last3.Value]);
                    args[last3.Value] = $"FromRegisterIndex(byte{i} & 0b111, {size})";
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
            this.Indented().AppendLine("length = this.Commit();");
            this.Indented().AppendLine($"return new Instructions.{name}({argsStr});");

            if (haveToCloseIf)
            {
                // Close the if caused by ModRM parse
                this.Unindent();
                this.Indented().AppendLine("}");
            }
        }

        private void BuildParserTree()
        {
            // Build the tree for the parser
            var root = new ParseNode(MatchType.None);
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

        private static bool SupportsOperand(Operand operand) => SupportedOperands.Contains(operand.Type);

        private static readonly string[] SupportedOperands = new[]
        {
            "1", "3",
            "al", "cl", "ax", "eax", "rax",
            "r8", "r16", "r32", "r64",
            "m",
            "m8", "m16", "m32", "m64", "m128", "m256", "m512",
            "imm8", "imm16", "imm32", "imm64",
            "rel8", "rel32",
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

        private static string GetDataWidthForOperand(Operand operand) => operand.Type switch
        {
            "m" => "null",
            "r8" or "m8" => "DataWidth.Byte",
            "r16" or "m16" => "DataWidth.Word",
            "r32" or "m32" => "DataWidth.Dword",
            "r64" or "m64" => "DataWidth.Qword",
            _ => throw new NotSupportedException(),
        };

        private static string GetDataWidthForSize(int size) => size switch
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
