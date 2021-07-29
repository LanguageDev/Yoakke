// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Platform.X86.Generator.Model;

namespace Yoakke.Platform.X86.Generator
{
    /// <summary>
    /// Generates the fluent writer API for the instructions.
    /// </summary>
    public static class WriterGenerator
    {
        /// <summary>
        /// Generates writer API for the all <see cref="Instruction"/>s specified.
        /// </summary>
        /// <param name="instructions">The set of <see cref="Instruction"/>s to generate for.</param>
        /// <returns>The generated writer flow API methods.</returns>
        public static string GenerateIsaWriters(ISet<Instruction> instructions)
        {
            var writers = new StringBuilder();
            foreach (var instruction in instructions)
            {
                try
                {
                    var source = GenerateInstructionWriters(instruction);
                    writers.Append(source);
                }
                catch (NotSupportedException)
                {
                    // Pass
                }
            }
            return writers.ToString();
        }

        /// <summary>
        /// Generates writers for a single <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to generate for.</param>
        /// <returns>The generated API methods.</returns>
        public static string GenerateInstructionWriters(Instruction instruction)
        {
            // Our first order of business is only keep the instruction forms that we support all operands for
            var supportedForms = instruction.Forms.Where(f => f.Operands.All(ClassGenerator.IsOperandSupported)).ToList();
            if (supportedForms.Count == 0) throw new NotSupportedException();

            var result = new StringBuilder();

            var className = Capitalize(instruction.Name);

            // Calculate how many properties we have
            var properties = ClassGenerator.GenerateProperties(supportedForms);

            // Generate the constructors into the class
            for (var paramCount = 0; paramCount <= properties.Count; ++paramCount)
            {
                // Check if we even need a builder with 'paramCount' no. parameters
                var form = supportedForms.Where(f => f.Operands.Count == paramCount).ToList();
                if (form.Count == 0) continue;

                // Yes we do need it, infer some names for us
                var ctorParams = ClassGenerator.GenerateProperties(form);

                result.AppendLine();
                // Doc comment
                result
                    .AppendLine("/// <summary>")
                    .AppendLine($"/// Writes a new {instruction.Name} instruction to the underlying assembly.")
                    .AppendLine("/// </summary>");
                // Parameter doc comments
                foreach (var param in ctorParams.Append(new(-1, "comment", "The optional inline comment.")))
                {
                    result.AppendLine($"/// <param name=\"{param.Name.ToLower()}\">{param.Docs}</param>");
                }
                // Return doc comment
                result.AppendLine("/// <returns>This instance to chain calls.</returns>");
                // Implementation
                result.Append($"public AssemblyBuilder {className}(");
                // Parameters
                result.Append(string.Join(string.Empty, ctorParams.Select(param => $"IOperand {param.Name.ToLower()}, ")));
                // Comment param
                result.Append("string? comment = null");
                result.AppendLine(") =>");
                // Write the instruction
                result
                    .Append($"    this.Write(new Instructions.{className}(")
                    .Append(string.Join(string.Empty, ctorParams.Select(p => $"{p.Name.ToLower()}, ")))
                    .AppendLine("comment));");
            }

            return result.ToString();
        }

        private static string Capitalize(string name) => $"{char.ToUpper(name[0])}{name[1..].ToLower()}";
    }
}
