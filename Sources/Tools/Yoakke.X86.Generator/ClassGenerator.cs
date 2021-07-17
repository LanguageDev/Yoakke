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
            var properties = GenerateProperties(instruction.Forms);

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
                var ctorParams = GenerateProperties(instruction.Forms.Where(f => f.Operands.Count == paramCount).ToList());

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

                    result.Append($"IOperand {param.Name.ToLower()}");
                }
                // Body
                result
                    .AppendLine(")")
                    .AppendLine("    {")
                    .AppendLine($"        this.Operands = new[] {{ {string.Join(", ", ctorParams.Select(p => p.Name.ToLower()))} }};")
                    .AppendLine("    }");
            }

            result.AppendLine("}");

            return result.ToString();
        }

        private static IReadOnlyList<OperandProperty> GenerateProperties(IReadOnlyList<InstructionForm> relevantForms)
        {
            Debug.Assert(relevantForms.Count > 0, "Must be at least one instruction form that has this many operands.");

            if (relevantForms.All(f => f.Operands.Count == 1))
            {
                // Every form has exactly one operand
                // For now we will just call that 'Operand'
                return new OperandProperty[]
                {
                    new()
                    {
                        OperandIndex = 0,
                        Docs = "The operand.",
                        Name = "Operand",
                    },
                };
            }

            if (relevantForms.All(f => f.Operands.Count == 2))
            {
                // Every form has exactly 2 operands, might be a target <- source pattern, chech that
                if (relevantForms.All(f => f.Operands[0].IsOutput && !f.Operands[1].IsOutput))
                {
                    // For a little extra readability, if the first operand is an input too, we can help that with some docs
                    if (relevantForms.All(f => f.Operands[0].IsInput))
                    {
                        // It's an inout <- in form, like ADD
                        return new OperandProperty[]
                        {
                            new()
                            {
                                OperandIndex = 0,
                                Docs = "The first input (and output) operand.",
                                Name = "Target",
                            },
                            new()
                            {
                                OperandIndex = 1,
                                Docs = "The second input operand.",
                                Name = "Source",
                            },
                        };
                    }
                    else
                    {
                        // It's an out <- in form, like MOV
                        return new OperandProperty[]
                        {
                            new()
                            {
                                OperandIndex = 0,
                                Docs = "The output operand.",
                                Name = "Destination",
                            },
                            new()
                            {
                                OperandIndex = 1,
                                Docs = "The input operand.",
                                Name = "Source",
                            },
                        };
                    }
                }
            }

            // By default, we just generate numbered operands
            var properties = new List<OperandProperty>();
            for (var i = 0; i < relevantForms.Count; ++i)
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
