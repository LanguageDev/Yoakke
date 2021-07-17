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
        private record OperandProperty(int OperandIndex, string Name, string Docs);

        /// <summary>
        /// Generates the classes for <see cref="Instructions"/> in the given <see cref="InstructionSet"/>.
        /// </summary>
        /// <param name="instructionSet">The <see cref="InstructionSet"/> to generate classes for.</param>
        /// <param name="generated">The set of <see cref="Instruction"/>s gets written here, that
        /// generated a class. Unsupported instructions are not written here.</param>
        /// <returns>The code for all of the class definitions.</returns>
        public static string GenerateIsaClasses(InstructionSet instructionSet, out ISet<Instruction> generated)
        {
            generated = new HashSet<Instruction>();
            var classes = new StringBuilder();
            foreach (var instruction in instructionSet.Instructions)
            {
                try
                {
                    var source = GenerateInstructionClass(instruction);
                    classes.AppendLine(source);
                    generated.Add(instruction);
                }
                catch (NotSupportedException)
                {
                    // Pass
                }
            }
            return classes.ToString();
        }

        /// <summary>
        /// Generates code for a single <see cref="Instruction"/> class.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate code for.</param>
        /// <returns>The built class code for <paramref name="instruction"/>.</returns>
        public static string GenerateInstructionClass(Instruction instruction)
        {
            // Our first order of business is only keep the instruction forms that we support all operands for
            var supportedForms = instruction.Forms.Where(f => f.Operands.All(IsOperandSupported)).ToList();
            if (supportedForms.Count == 0) throw new NotSupportedException();

            var result = new StringBuilder();

            // Class doc comment
            result
                .AppendLine("/// <summary>")
                .AppendLine($"/// {instruction.Summary}.")
                .AppendLine("/// </summary>");

            // Class start
            var className = Capitalize(instruction.Name);
            result
                .AppendLine($"public class {className} : IInstruction")
                .AppendLine("{");

            // For now we store all instruction operands in a read-only list
            result.AppendLine("    /// <inheritdoc/>");
            result.AppendLine($"    public IReadOnlyList<IOperand> Operands {{ get; }}");

            // The comment property
            result.AppendLine();
            result.AppendLine("    /// <inheritdoc/>");
            result.AppendLine("    public string? Comment { get; init; }");

            // Infer properties
            var properties = GenerateProperties(supportedForms);

            // Write properties into the class
            foreach (var prop in properties)
            {
                result.AppendLine();

                // Doc comment
                result
                    .AppendLine("    /// <summary>")
                    .AppendLine($"    /// {prop.Docs}")
                    .AppendLine("    /// </summary>");
                // Getter
                result.AppendLine($"    public IOperand {prop.Name} => this.Operands[{prop.OperandIndex}];");
            }

            // Generate the constructors into the class
            for (var paramCount = 0; paramCount <= properties.Count; ++paramCount)
            {
                // Check if we even need a constructor with 'paramCount' no. parameters
                var form = supportedForms.Where(f => f.Operands.Count == paramCount).ToList();
                if (form.Count == 0) continue;

                // Yes we do need it, infer some names for us
                var ctorParams = GenerateProperties(form);

                // Inject the doc comment parameter
                ctorParams = ctorParams.Append(new(-1, "comment", "The optional inline comment.")).ToList();

                result.AppendLine();

                // Ctor doc comment
                result
                    .AppendLine("    /// <summary>")
                    .AppendLine($"    /// Initializes a new instance of the <see cref=\"{className}\"/> class.")
                    .AppendLine("    /// </summary>");
                // Parameter doc comments
                foreach (var param in ctorParams)
                {
                    result.AppendLine($"    /// <param name=\"{param.Name.ToLower()}\">{param.Docs}</param>");
                }

                // Ctor code
                result.Append($"    public {className}(");
                // Parameters
                var first = true;
                foreach (var param in ctorParams)
                {
                    if (!first) result.Append(", ");
                    first = false;

                    // Comment, default it
                    if (param.OperandIndex == -1) result.Append($"string? {param.Name.ToLower()} = null");
                    // Regular operand
                    else result.Append($"IOperand {param.Name.ToLower()}");
                }
                // Body
                result
                    .AppendLine(")")
                    .AppendLine("    {");
                if (ctorParams.Count > 1)
                {
                    // We have more than just the comment parameter
                    var args = string.Join(", ", ctorParams.SkipLast(1).Select(p => p.Name.ToLower()));
                    result.AppendLine($"        this.Operands = new[] {{ {args} }};");
                }
                else
                {
                    // No operands
                    result.AppendLine($"        this.Operands = Array.Empty<IOperand>();");
                }
                // Also assign the comment, close ctor
                result
                    .AppendLine("        this.Comment = comment;")
                    .AppendLine("    }");
            }

            result.AppendLine("}");

            return result.ToString();
        }

        private static IReadOnlyList<OperandProperty> GenerateProperties(IReadOnlyList<InstructionForm> relevantForms)
        {
            Debug.Assert(relevantForms.Count > 0, "Must be at least one instruction form that has this many operands.");

            if (relevantForms.Max(f => f.Operands.Count) == 1)
            {
                // Every form has exactly 0 or 1 operand
                // For now we will just call that 'Operand'
                return new OperandProperty[] { new(0, "Operand", "The operand.") };
            }

            if (relevantForms.All(f => f.Operands.Count == 2))
            {
                // Every form has exactly 2 operands, might be a target <- source pattern, chech that
                if (relevantForms.All(f =>
                    // Output <- !Output
                       (f.Operands[0].IsOutput && !f.Operands[1].IsOutput)
                    // !Input <- Input
                    || (!f.Operands[0].IsInput && f.Operands[1].IsInput)))
                {
                    // For a little extra readability, if the first operand is an input too, we can help that with some docs
                    if (relevantForms.All(f => f.Operands[0].IsInput))
                    {
                        // It's an inout <- in form, like ADD
                        return new OperandProperty[]
                        {
                            new(0, "Target", "The first input (and output) operand."),
                            new(1, "Source", "The second input operand."),
                        };
                    }
                    else
                    {
                        // It's an out <- in form, like MOV
                        return new OperandProperty[]
                        {
                            new(0, "Destination", "The output operand."),
                            new(1, "Source", "The input operand."),
                        };
                    }
                }

                // We just return a little first, second pair
                return new OperandProperty[]
                {
                    new(0, "First", "The first operand."),
                    new(1, "Second", "The second operand."),
                };
            }

            // By default, we just generate numbered operands
            var maxNumberOfOperands = relevantForms.Max(f => f.Operands.Count);
            var properties = new List<OperandProperty>();
            for (var i = 0; i < maxNumberOfOperands; ++i)
            {
                properties.Add(new(i, $"Operand{i + 1}", $"The {i + 1}{GetNumberSuffix(i + 1)} operand."));
            }
            return properties;
        }

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name.Substring(1).ToLower()}";

        private static string GetNumberSuffix(int i) => i switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };

        private static bool IsOperandSupported(Operand operand) => SupportedOperands.Contains(operand.Type);

        private static readonly string[] SupportedOperands = new[]
        {
            "1", "3",
            "al", "cl", "ax", "eax", "rax",
            "imm8", "imm16", "imm32", "imm64",
            "r8", "r16", "r32", "r64",
            "m",
            "m8", "m16", "m32", "m64",
            "rel8", "rel32",
        };
    }
}
