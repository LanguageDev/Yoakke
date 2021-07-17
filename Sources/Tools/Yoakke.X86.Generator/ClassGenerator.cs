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
        private struct OperandProperty
        {
            public int OperandIndex { get; set; }

            public string Name { get; set; }

            public string Docs { get; set; }
        }

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
            result.AppendLine($"    public IReadOnlyList<IOperand> Operands {{ get; }}");

            // Infer properties
            var properties = GenerateProperties(instruction);

            // Write properties into the class
            foreach (var prop in properties)
            {
                result.AppendLine();

                // Doc comment
                result
                    .AppendLine("    /// <summary>")
                    .AppendLine($"    /// {prop.Docs}")
                    .AppendLine("    /// </summary>");
                // Getter and setter
                result
                    .AppendLine($"    public IOperand {prop.Name}")
                    .AppendLine("    {")
                    .AppendLine($"        get => this.Operands[{prop.OperandIndex}];")
                    .AppendLine($"        set => this.Operands[{prop.OperandIndex}] = value;")
                    .AppendLine("    }");
            }

            // Generate the constructors into the class
            for (var paramCount = 0; paramCount <= properties.Count; ++paramCount)
            {
                // Check if we even need a constructor with 'paramCount' no. parameters
                if (!instruction.Forms.Any(f => f.Operands.Count == paramCount)) continue;

                // Yes we do need it, infer some names for us
                var ctorParams = GenerateConstructorParams(instruction, paramCount);

                result.AppendLine();

                // Ctor doc comment
                result
                    .AppendLine("    /// <summary>")
                    .AppendLine($"    /// Initializes a new instance of the <see cref=\"{className}\"/> class.")
                    .AppendLine("    /// </summary>");
                // Parameter doc comments
                foreach (var param in ctorParams)
                {
                    result.AppendLine($"    /// <param name=\"{param.Name}\">{param.Docs}</param>");
                }

                // Ctor code
                result.Append($"    public {className}(");
                // Parameters
                var first = true;
                foreach (var param in ctorParams)
                {
                    if (!first) result.Append(", ");
                    first = false;

                    result.Append($"IOperand {param.Name}");
                }
                // Body
                result
                    .AppendLine(")")
                    .AppendLine("    {")
                    .AppendLine($"        this.Operands = new[] {{ {string.Join(", ", ctorParams.Select(p => p.Name))} }};")
                    .AppendLine("    }");
            }

            result.AppendLine("}");

            return result.ToString();
        }

        private static IReadOnlyList<OperandProperty> GenerateProperties(Instruction instruction)
        {
            var properties = new List<OperandProperty>();

            // Find out how many properties we need
            var propCount = instruction.Forms.Max(f => f.Operands.Count);

            // TODO: Make this a bit smarter
            // For now we just generate Operand1, Operand2, ...
            for (var i = 0; i < propCount; ++i)
            {
                properties.Add(new()
                {
                    OperandIndex = i,
                    Name = $"Operand{i + 1}",
                    Docs = $"The {i + 1}{GetNumberSuffix(i + 1)} operand.",
                });
            }

            return properties;
        }

        private static IReadOnlyList<OperandProperty> GenerateConstructorParams(Instruction instruction, int operandCount)
        {
            var properties = new List<OperandProperty>();

            // TODO: Make this a bit smarter
            // For now we just generate Operand1, Operand2, ...
            for (var i = 0; i < operandCount; ++i)
            {
                properties.Add(new()
                {
                    OperandIndex = i,
                    Name = $"operand{i + 1}",
                    Docs = $"The {i + 1}{GetNumberSuffix(i + 1)} operand.",
                });
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
    }
}
