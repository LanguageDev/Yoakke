// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Yoakke.LSP.Generator
{
    internal class Program
    {
        private const string JsonPropertyAttribute = "JsonProperty";
        private const string JsonEnumValueAttribute = "EnumMember";
        private static readonly string[] knownInterfaces = new string[]
        {
            "WorkDoneProgressParams", "WorkDoneProgressOptions",
            "TextDocumentRegistrationOptions",
            "StaticRegistrationOptions",
        };
        private static readonly List<string> typeDefinitions = new();

        private static void Main(string[] args)
        {
            var lexer = new TsLexer(Console.In);
            var parser = new TsParser(lexer);
            while (true)
            {
                var parseResult = parser.ParseDefinition();
                if (parseResult.IsError)
                {
                    Console.WriteLine("Failed to parse!");
                    var err = parseResult.Error;
                    foreach (var element in err.Elements.Values)
                    {
                        Console.WriteLine($"  expected {string.Join(" or ", element.Expected)} while parsing {element.Context}");
                    }
                    Console.WriteLine($"  but got {(err.Got == null ? "end of input" : err.Got.Text)}");
                    break;
                }
                var ast = parseResult.Ok.Value;
                Translate(ast);

                Console.WriteLine("====================================================");
                Console.WriteLine("Generated types:");
                Console.WriteLine();

                foreach (var typeDef in typeDefinitions)
                {
                    Console.WriteLine();
                    Console.WriteLine(typeDef);
                }

                Console.WriteLine("====================================================");

                typeDefinitions.Clear();
            }
        }

        private static void Translate(DefBase def)
        {
            switch (def)
            {
            case InterfaceDef i:
                Translate(i);
                break;
            case NamespaceDef n:
                Translate(n);
                break;
            default: throw new InvalidOperationException();
            }
        }

        private static void Translate(InterfaceDef interfaceDef)
        {
            var result = new StringBuilder();
            var interfaces = interfaceDef.Bases.Intersect(knownInterfaces).ToList();
            var baseClasses = interfaceDef.Bases.Except(interfaces).ToList();
            if (interfaceDef.Docs != null) result.AppendLine(TranslateDocComment("", interfaceDef.Docs));
            result.Append($"public class {interfaceDef.Name}");
            if (baseClasses.Count > 1) throw new NotSupportedException();
            var toExtend = baseClasses.Concat(interfaces.Select(i => $"I{i}")).ToList();
            if (toExtend.Count > 0) result.Append(" : ").Append(string.Join(", ", toExtend));
            result.AppendLine();
            result.AppendLine("{");
            foreach (var field in interfaceDef.Fields) result.AppendLine(Translate(field));
            result.AppendLine("}");
            typeDefinitions.Add(result.ToString());
        }

        private static void Translate(NamespaceDef namespaceDef)
        {
            var result = new StringBuilder();
            if (namespaceDef.Docs != null) result.AppendLine(TranslateDocComment("", namespaceDef.Docs));
            result.AppendLine($"public enum {namespaceDef.Name}");
            result.AppendLine("{");
            foreach (var field in namespaceDef.Fields) result.AppendLine(Translate(field));
            result.AppendLine("}");
            typeDefinitions.Add(result.ToString());
        }

        private static string Translate(string? hintName, TypeNode type) => type switch
        {
            TypeNode.Ident id => TranslateTypeName(id.Name),
            TypeNode.Array arr => $"IReadOnlyList<{Translate(null, arr.ElementType)}>",
            TypeNode.Object obj => TranslateObject(hintName, obj),
            TypeNode.Or or => TranslateOr(hintName, or),
            _ => throw new InvalidOperationException(),
        };

        private static string TranslateOr(string? hintName, TypeNode.Or or)
        {
            if (or.Alternatives.Count == 0) throw new InvalidOperationException();

            if (or.Alternatives.Any(alt => alt is TypeNode.Ident id && id.Name == "null"))
            {
                // Basically a nullable type, take it out, translate without it, add nullability
                var nonNullAlternatives = or.Alternatives
                    .Where(alt => alt is not TypeNode.Ident id || id.Name != "null")
                    .ToArray();
                var result = nonNullAlternatives.Length == 1
                    ? Translate(hintName, nonNullAlternatives[0])
                    : Translate(hintName, new TypeNode.Or(nonNullAlternatives));
                return WithOptional(result);
            }

            if (or.Alternatives.Count == 2 || or.Alternatives.Count == 3)
            {
                // Generate an either type
                var alts = string.Join(", ", or.Alternatives.Select(alt => Translate(hintName, alt)));
                return $"Either<{alts}>";
            }

            throw new NotImplementedException();
        }

        private static string TranslateObject(string? hintName, TypeNode.Object obj)
        {
            if (hintName == null) throw new ArgumentNullException(nameof(hintName));
            var result = new StringBuilder();
            var className = hintName;
            result.AppendLine($"public class {hintName}");
            result.AppendLine("{");
            foreach (var field in obj.Fields) result.AppendLine(Translate(field));
            result.AppendLine("}");
            typeDefinitions.Add(result.ToString());
            return className;
        }

        private static string Translate(InterfaceField field)
        {
            var result = new StringBuilder();
            var fieldName = Capitalize(field.Name);
            if (field.Docs != null) result.AppendLine(TranslateDocComment("    ", field.Docs));
            var propExtra = "";
            if (field.Optional) propExtra += ", NullValueHandling = NullValueHandling.Ignore";
            result.AppendLine($"    [{JsonPropertyAttribute}(\"{field.Name}\"{propExtra})]");
            result.Append("    public ");
            var typeName = Translate(fieldName, field.Type);
            if (field.Optional) typeName = WithOptional(typeName);
            result.Append(typeName);
            result.Append(' ');
            result.Append(fieldName);
            result.Append(" { get; set; }");
            return result.ToString();
        }

        private static string Translate(NamespaceField field)
        {
            var result = new StringBuilder();
            var fieldName = Capitalize(field.Name);
            if (field.Docs != null) result.AppendLine(TranslateDocComment("    ", field.Docs));
            if (field.Value is string strValue)
            {
                result.AppendLine($"    [{JsonEnumValueAttribute}(Value = \"{strValue}\")]");
            }
            result.Append("    ");
            result.Append(fieldName);
            if (field.Value is int intValue)
            {
                result.Append(" = ");
                result.Append(intValue);
            }
            result.Append(',');
            return result.ToString();
        }

        private static string TranslateTypeName(string name)
        {
            if (knownInterfaces.Contains(name)) throw new InvalidOperationException();
            return name switch
            {
                "boolean" => "bool",
                "any" => "object",
                "integer" => "int",
                "uinteger" => "uint",
                _ => name,
            };
        }

        private static string TranslateDocComment(string prefix, string docComment)
        {
            var result = new StringBuilder();
            var doc = DocComment.FromComment(docComment);
            if (doc.Summary != null)
            {
                result.Append(prefix);
                result.AppendLine("/// <summary>");
                // Divide up into lines
                var lineReader = new StringReader(doc.Summary);
                while (true)
                {
                    var line = lineReader.ReadLine();
                    if (line == null) break;
                    result.Append(prefix);
                    result.Append("/// ");
                    result.AppendLine(line);
                }
                result.Append(prefix);
                result.AppendLine("/// </summary>");
            }
            if (doc.Since != null)
            {
                var versionParts = doc.Since.Split('.');
                result.Append(prefix);
                result.Append("[Since(");
                result.Append(string.Join(", ", versionParts));
                result.AppendLine(")]");
            }
            if (doc.Deprecation != null)
            {
                result.Append(prefix);
                result.Append("[Obsolete(\"");
                result.Append(doc.Deprecation);
                result.AppendLine("\")]");
            }
            return result.ToString().TrimEnd();
        }

        private static string Capitalize(string s) => string.IsNullOrEmpty(s)
            ? s
            : char.ToUpper(s[0]) + s.Substring(1);

        private static string WithOptional(string type) => type.EndsWith('?') ? type : $"{type}?";
    }
}
