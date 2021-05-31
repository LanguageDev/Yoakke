using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yoakke.Lexer;

namespace Yoakke.LSP.Generator
{
    class Program
    {
        static List<string> typeDefinitions = new();

        static void Main(string[] args)
        {
            var lexer = new TsLexer(Console.In);
            var parser = new TsParser(lexer);
            while (true)
            {
                var parseResult = parser.ParseInterface();
                if (parseResult.IsError)
                {
                    Console.WriteLine("Failed to parse!");
                    break;
                }
                var ast = parseResult.Ok.Value;
                Translate(ast);
            }

            Console.WriteLine("====================================================");
            Console.WriteLine();
            Console.WriteLine("Generated types:");

            foreach (var typeDef in typeDefinitions)
            {
                Console.WriteLine();
                Console.WriteLine(typeDef);
            }
        }

        static void Translate(InterfaceDef interfaceDef)
        {
            var result = new StringBuilder();
            // TODO: Subclassing
            if (interfaceDef.Docs != null) result.AppendLine(TranslateDocComment("", interfaceDef.Docs));
            result.AppendLine($"public class {interfaceDef.Name}");
            result.AppendLine("{");
            foreach (var field in interfaceDef.Fields) result.AppendLine(Translate(field));
            result.AppendLine("}");
            typeDefinitions.Add(result.ToString());
        }

        static string Translate(string? hintName, TypeNode type) => type switch
        {
            TypeNode.Ident id => TranslateTypeName(id.Name),
            TypeNode.Array arr => $"IReadOnlyList<{Translate(null, arr.ElementType)}>",
            TypeNode.Object obj => TranslateObject(hintName, obj),
            TypeNode.Or => throw new NotImplementedException(),
            _ => throw new InvalidOperationException(),
        };

        static string TranslateObject(string? hintName, TypeNode.Object obj)
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

        static string Translate(InterfaceField field)
        {
            var result = new StringBuilder();
            var fieldName = Capitalize(field.Name);
            if (field.Docs != null) result.AppendLine(TranslateDocComment("    ", field.Docs));
            result.AppendLine($"    [JsonPropertyName(\"{field.Name}\")]");
            result.Append("    public ");
            result.Append(Translate(fieldName, field.Type));
            if (field.Optional) result.Append('?');
            result.Append(' ');
            result.Append(fieldName);
            result.Append(" { get; set; }");
            return result.ToString();
        }

        static string TranslateTypeName(string name) => name switch
        {
            "boolean" => "bool",
            _ => name,
        };

        static string TranslateDocComment(string prefix, string docComment)
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
            if (doc.Deprecation != null)
            {
                result.Append(prefix);
                result.Append("[ObsoleteAttribute(\"");
                result.Append(doc.Deprecation);
                result.AppendLine("\")]");
            }
            // TODO: Since?
            return result.ToString().TrimEnd();
        }

        static string Capitalize(string s) => string.IsNullOrEmpty(s)
            ? s
            : char.ToUpper(s[0]) + s.Substring(1);
    }
}
