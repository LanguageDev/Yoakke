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
        static readonly string JsonPropertyAttribute = "JsonProperty";
        static readonly string JsonEnumValueAttribute = "EnumMember";

        static List<string> typeDefinitions = new();

        static void Main(string[] args)
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

        static void Translate(DefBase def)
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

        static void Translate(NamespaceDef namespaceDef)
        {
            var result = new StringBuilder();
            // TODO: Subclassing
            if (namespaceDef.Docs != null) result.AppendLine(TranslateDocComment("", namespaceDef.Docs));
            result.AppendLine($"public enum {namespaceDef.Name}");
            result.AppendLine("{");
            foreach (var field in namespaceDef.Fields) result.AppendLine(Translate(field));
            result.AppendLine("}");
            typeDefinitions.Add(result.ToString());
        }

        static string Translate(string? hintName, TypeNode type) => type switch
        {
            TypeNode.Ident id => TranslateTypeName(id.Name),
            TypeNode.Array arr => $"IReadOnlyList<{Translate(null, arr.ElementType)}>",
            TypeNode.Object obj => TranslateObject(hintName, obj),
            TypeNode.Or or => TranslateOr(or),
            _ => throw new InvalidOperationException(),
        };

        static string TranslateOr(TypeNode.Or or)
        {
            if (or.Alternatives.Count == 0) throw new InvalidOperationException();

            if (or.Alternatives.Any(alt => alt is TypeNode.Ident id && id.Name == "null"))
            {
                // Basically a nullable type, take it out, translate without it, add nullability
                var nonNullAlternatives = or.Alternatives
                    .Where(alt => alt is not TypeNode.Ident id || id.Name != "null")
                    .ToArray();
                var result = nonNullAlternatives.Length == 1
                    ? Translate(null, nonNullAlternatives[0])
                    : Translate(null, new TypeNode.Or(nonNullAlternatives));
                return WithOptional(result);
            }

            throw new NotImplementedException();
        }

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
            string propExtra = "";
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

        static string Translate(NamespaceField field)
        {
            var result = new StringBuilder();
            var fieldName = Capitalize(field.Name);
            if (field.Docs != null) result.AppendLine(TranslateDocComment("    ", field.Docs));
            if (field.Value is string strValue)
            {
                result.AppendLine($"    [{JsonEnumValueAttribute}(\"{strValue}\")]");
            }
            result.Append("    ");
            result.Append(fieldName);
            if (field.Value is int intValue)
            {
                result.Append(" = ");
                result.Append(intValue);
            }
            result.Append(",");
            return result.ToString();
        }

        static string TranslateTypeName(string name) => name switch
        {
            "boolean" => "bool",
            "any" => "object",
            "integer" => "int",
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

        static string WithOptional(string type) => type.EndsWith('?') ? type : $"{type}?";
    }
}
