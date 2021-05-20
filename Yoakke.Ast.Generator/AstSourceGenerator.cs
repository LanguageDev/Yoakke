using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;
using Yoakke.SourceGenerator.Common;

namespace Yoakke.Ast.Generator
{
    [Generator]
    public class AstSourceGenerator : GeneratorBase
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclSyntax
                    && classDeclSyntax.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(classDeclSyntax);
                }
            }
        }

        private Dictionary<string, MetaNode> rootNodes = new();
        private Dictionary<string, MetaNode> allNodes = new();

        public AstSourceGenerator() 
            : base("Yoakke.Ast.Generator")
        {
        }

        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            var receiver = (SyntaxReceiver)syntaxReceiver;

            RequireLibrary("Yoakke.Ast");

            BuildMetaNodes(receiver.CandidateClasses);

            // Now generate each node source
            foreach (var node in allNodes.Values) GenerateNodeSource(node);
        }

        private void BuildMetaNodes(IList<ClassDeclarationSyntax> classDeclarations)
        {
            // Collect only the classes that have the AstAttribute
            var astNodeSymbols = new HashSet<INamedTypeSymbol>();
            foreach (var syntax in classDeclarations)
            {
                var model = Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (!HasAttribute(symbol!, TypeNames.AstAttribute)) continue;
                if (!RequirePartial(syntax)) continue;
                astNodeSymbols.Add(symbol!);
            }

            // Select all the root nodes
            // They are all the nodes without a base class or with a base class that's not an AST node
            var rootNodeSymbols = astNodeSymbols
                .Where(sym => sym.BaseType == null || !HasAttribute(sym.BaseType, TypeNames.AstAttribute))
                .ToHashSet();
            rootNodes = rootNodeSymbols
                .Select(sym => MakeMetaNode(sym, null))
                .ToDictionary(n => n.Name);
            // Clone them to all nodes
            allNodes = rootNodes.ToDictionary(n => n.Key, n => n.Value);

            // Remove them from all symbols
            astNodeSymbols = astNodeSymbols
                .Except(rootNodeSymbols)
                .ToHashSet();

            // Now loop until all nodes could be attached somewhere
            while (astNodeSymbols.Count > 0)
            {
                var toRemove = new HashSet<INamedTypeSymbol>();
                foreach (var symbol in astNodeSymbols)
                {
                    if (!allNodes.TryGetValue(symbol.BaseType!.Name, out var parentNode)) continue;
                    // We found this node's parent in out existing nodes
                    var node = MakeMetaNode(symbol, parentNode);
                    allNodes.Add(node.Name, node);
                    toRemove.Add(symbol);
                }
                // Remove all found nodes
                astNodeSymbols = astNodeSymbols
                    .Except(toRemove)
                    .ToHashSet();
                // If we removed nothing, there's an error
                if (toRemove.Count == 0)
                {
                    // TODO: Error
                    break;
                }
            }
        }

        private MetaNode MakeMetaNode(INamedTypeSymbol symbol, MetaNode? parent)
        {
            var node = new MetaNode(symbol, parent);
            if (parent != null) parent.Children.Add(node.Name, node);

            if (TryGetAttribute(symbol, TypeNames.ImplementEqualityAttribute, out var equalityAttr))
            {
                node.ImplementEquality =
                    equalityAttr.ConstructorArguments.Length == 0 || (bool?)equalityAttr.ConstructorArguments[0].Value == true;
            }

            return node;
        }

        private void GenerateNodeSource(MetaNode node)
        {
            var isRoot = node.Parent == null;
            // Generate this node
            var interfacesToImplement = new List<string>();
            interfacesToImplement.Add(TypeNames.IAstNode);
            if (isRoot)
            {
                // There are potentially things to implement
                if (node.ImplementEquality) interfacesToImplement.Add($"{TypeNames.IEquatable}<{node.Symbol.ToDisplayString()}>");
            }

            var fields = node.Symbol.GetMembers()
                .Where(member => !member.IsStatic)
                .OfType<IFieldSymbol>()
                .ToList();

            var extraDefinitions = new StringBuilder();

            // Generate implementation for ChildNodes
            var childNodes = fields.Where(f => HasAttribute(f.Type, TypeNames.AstAttribute)).ToList();
            extraDefinitions.AppendLine($"{TypeNames.IEnumerable}<{TypeNames.IAstNode}> {TypeNames.IAstNode}.ChildNodes");
            extraDefinitions.AppendLine("{ get {");
            extraDefinitions.AppendLine(string.Join(string.Empty, childNodes.Select(n => $"yield return {n.Name};")));
            if (childNodes.Count == 0) extraDefinitions.AppendLine("yield break;");
            extraDefinitions.AppendLine("} }");

            // Generate implementation for LeafObjects
            var leafObjects = fields.Except(childNodes).ToList();
            extraDefinitions.AppendLine($"{TypeNames.IEnumerable}<object> {TypeNames.IAstNode}.LeafObjects");
            extraDefinitions.AppendLine("{ get {");
            extraDefinitions.AppendLine(string.Join(string.Empty, leafObjects.Select(n => $"yield return {n.Name};")));
            if (leafObjects.Count == 0) extraDefinitions.AppendLine("yield break;");
            extraDefinitions.AppendLine("} }");

            // Generate implementation for pretty-print
            // TODO: Stub for now
            extraDefinitions.AppendLine($"string {TypeNames.IAstNode}.PrettyPrint({TypeNames.PrettyPrintFormat} format)");
            extraDefinitions.AppendLine("{");
            extraDefinitions.AppendLine("    return string.Empty;");
            extraDefinitions.AppendLine("}");

            // Generate ctor
            var ctorSource = $@"
public {node.Name}({string.Join(", ", fields.Select(f => $"{f.Type.ToDisplayString()} {f.Name}"))}) {{
    {string.Join(string.Empty, fields.Select(f => $"this.{f.Name} = {f.Name};"))};
}}
";
            var emptyCtorSource = fields.Count == 0 ? string.Empty : $"protected {node.Name}() {{ }}";

            // Extra stuff like interface implementations
            if (isRoot && node.ImplementEquality)
            {
                // A root node that needs equality always defines object.Equals
                extraDefinitions.AppendLine($"public override bool Equals(object other) => Equals(other as {node.Name});");
            }

            if (node.IsAbstract)
            {
                if (isRoot && node.ImplementEquality)
                {
                    // Define equality abstractly
                    extraDefinitions.AppendLine($"public abstract override bool Equals({node.Name} other);");
                }
            }
            else
            {
                if (node.ImplementEquality)
                {
                    // Override equality and hash
                    var lastEquality = LastParentThat(node, n => n.ImplementEquality)!.Symbol.ToDisplayString();
                    var (eqCmp, hash) = GenerateEqualityElements(node.Symbol, fields);
                    
                    var comparisons = string.Join(" && ", eqCmp);
                    extraDefinitions.AppendLine($"public override bool Equals({lastEquality} o) =>");
                    extraDefinitions.AppendLine($"    o is {node.Name} other && {comparisons};");

                    extraDefinitions.AppendLine("public override int GetHashCode()");
                    extraDefinitions.AppendLine("{");
                    extraDefinitions.AppendLine($"    var hash = new {TypeNames.HashCode}();");
                    extraDefinitions.AppendLine(string.Join(string.Empty, hash));
                    extraDefinitions.AppendLine("    return hash.ToHashCode();");
                    extraDefinitions.AppendLine("}");
                }
            }

            // Surrounding crud
            var surroundingNamespace = node.Symbol.ContainingNamespace.ToDisplayString();
            var nestClassPrefix = new StringBuilder();
            var nestClassSuffix = new StringBuilder();
            foreach (var nest in node.Nesting)
            {
                nestClassPrefix.AppendLine($"partial class {nest} {{");
                nestClassSuffix.AppendLine("}");
            }

            var sourceCode = $@"
using System.Linq;

namespace {surroundingNamespace} {{
    {nestClassPrefix}
    partial class {node.Name} : {string.Join(", ", interfacesToImplement)} {{
        {emptyCtorSource}
        {ctorSource}
        {extraDefinitions}
    }}
    {nestClassSuffix}
}}
";
            AddSource($"{node.Symbol.ToDisplayString()}.Generated.cs", sourceCode);
        }

        private (List<string> Equality, List<string> Hash) GenerateEqualityElements(ISymbol symbol, List<IFieldSymbol> fields)
        {
            var readOnlyListInterface = LoadSymbol(TypeNames.IReadOnlyList);
            var equality = new List<string>();
            var hash = new List<string>();
            foreach (var field in fields)
            {
                if (field.Type.ImplementsInterface(readOnlyListInterface))
                {
                    // Use .SequenceEquals and add each element to hash one by one
                    equality.Add($"this.{field.Name}.Count == other.{field.Name}.Count && this.{field.Name}.SequenceEquals(other.{field.Name})");
                    hash.Add($"foreach (var item in this.{field.Name}) hash.Add(item);");
                }
                else
                {
                    // Just call Equals and add to hash
                    equality.Add($"this.{field.Name}.Equals(other.{field.Name})");
                    hash.Add($"hash.Add(this.{field.Name});");
                }
            }
            return (equality, hash);
        }

        private static MetaNode? LastParentThat(MetaNode root, Predicate<MetaNode> pred)
        {
            if (root.Parent == null)
            {
                if (pred(root)) return root;
                return null;
            }
            if (!pred(root.Parent)) return root;
            return LastParentThat(root.Parent, pred);
        }
    }
}
