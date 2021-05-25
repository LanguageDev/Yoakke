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

        private enum FieldKind 
        { 
            Leaf,
            Subnode,
            LeafList,
            SubnodeList,
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

            //Debugger.Launch();
            // Generate visitor contents
            foreach (var node in rootNodes.Values) GenerateVisitorSource(node);
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

            var fields = CategorizeAllFields(node.Symbol);

            var extraDefinitions = new StringBuilder();

            // Generate implementation for ChildNodes, which yields subnodes and subnode lists
            var subnodeLeaves = fields.Where(f => f.Kind == FieldKind.Subnode).Select(f => f.Symbol).ToList();
            var subnodeLists = fields.Where(f => f.Kind == FieldKind.SubnodeList).Select(f => f.Symbol).ToList();
            extraDefinitions.AppendLine($"{TypeNames.IEnumerable}<{TypeNames.IAstNode}> {TypeNames.IAstNode}.ChildNodes");
            extraDefinitions.AppendLine("{ get {");
            foreach (var subnode in subnodeLeaves) extraDefinitions.AppendLine($"yield return this.{subnode.Name};");
            foreach (var subnodeList in subnodeLists) extraDefinitions.AppendLine($"foreach (var item in this.{subnodeList.Name}) yield return item;");
            if (subnodeLeaves.Count == 0 && subnodeLists.Count == 0) extraDefinitions.AppendLine("yield break;");
            extraDefinitions.AppendLine("} }");

            // Generate implementation for LeafObjects, which yields leaf members and leaf lists
            var leafObjects = fields.Where(f => f.Kind == FieldKind.Leaf).Select(f => f.Symbol).ToList();
            var leafLists = fields.Where(f => f.Kind == FieldKind.LeafList).Select(f => f.Symbol).ToList();
            extraDefinitions.AppendLine($"{TypeNames.IEnumerable}<object> {TypeNames.IAstNode}.LeafObjects");
            extraDefinitions.AppendLine("{ get {");
            foreach (var leaf in leafObjects) extraDefinitions.AppendLine($"yield return this.{leaf.Name};");
            foreach (var leafList in leafLists) extraDefinitions.AppendLine($"foreach (var item in this.{leafList.Name}) yield return item;");
            if (leafObjects.Count == 0 && leafLists.Count == 0) extraDefinitions.AppendLine("yield break;");
            extraDefinitions.AppendLine("} }");

            // Generate implementation for pretty-print
            extraDefinitions.AppendLine($"string {TypeNames.IAstNode}.PrettyPrint({TypeNames.PrettyPrintFormat} format)");
            extraDefinitions.AppendLine("{");
            extraDefinitions.AppendLine($"    var result = new {TypeNames.StringBuilder}();");
            extraDefinitions.AppendLine("    this.PrettyPrintImpl(format, result, 0);");
            extraDefinitions.AppendLine("    return result.ToString();");
            extraDefinitions.AppendLine("}");
            // Pretty-print internals
            if (node.IsAbstract)
            {
                extraDefinitions.AppendLine($"protected abstract void PrettyPrintImpl({TypeNames.PrettyPrintFormat} format, {TypeNames.StringBuilder} builder, int indent);");
            }
            else
            {
                extraDefinitions.AppendLine($"protected override void PrettyPrintImpl({TypeNames.PrettyPrintFormat} format, {TypeNames.StringBuilder} builder, int indent)");
                extraDefinitions.AppendLine("{");
                // TODO: For now we only do XML
                extraDefinitions.AppendLine("builder.Append(' ', 2 * indent);");
                extraDefinitions.AppendLine($"builder.Append(\"<{node.Name}\");");
                if (leafObjects.Count > 0)
                {
                    // Leaves are attributes
                    foreach (var leaf in leafObjects)
                    {
                        extraDefinitions.AppendLine($"builder.Append($\" {leaf.Name}=\\\"{{{leaf.Name}}}\\\"\");");
                    }
                }
                extraDefinitions.AppendLine($"builder.AppendLine(\">\");");
                foreach (var leafList in leafLists)
                {
                    if ()
                }
                extraDefinitions.AppendLine("builder.Append(' ', 2 * indent);");
                extraDefinitions.AppendLine($"builder.AppendLine(\"<{node.Name} />\");");
                extraDefinitions.AppendLine("}");
            }

            // Generate ctor
            var ctorSource = $@"
public {node.Name}({string.Join(", ", fields.Select(f => $"{f.Symbol.Type.ToDisplayString()} {f.Symbol.Name}"))}) {{
    {string.Join(string.Empty, fields.Select(f => $"this.{f.Symbol.Name} = {f.Symbol.Name};"))};
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
                    // Define equality and hash abstractly
                    extraDefinitions.AppendLine($"public abstract bool Equals({node.Name} other);");
                    extraDefinitions.AppendLine("public abstract override int GetHashCode();");
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

        private void GenerateVisitorSource(MetaNode node)
        {
            var visitors = GenerateVisitors(node, new Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content, INamedTypeSymbol ReturnType)>());
            foreach (var visitor in visitors)
            {
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
namespace {surroundingNamespace} {{
    {nestClassPrefix}
    partial class {visitor.Value.Root.Name} {{
        public abstract class {visitor.Key} {{
            {visitor.Value.Content}
        }}
    }}
    {nestClassSuffix}
}}
";
                AddSource($"{node.Symbol.ToDisplayString()}.{visitor.Key}.Generated.cs", sourceCode);
            }
        }

        private Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content)> GenerateVisitors(
            MetaNode node, 
            IReadOnlyDictionary<string, (INamedTypeSymbol Root, StringBuilder Content, INamedTypeSymbol ReturnType)> visitors)
        {
            var readOnlyList = LoadSymbol(TypeNames.IReadOnlyList);

            // Get all visitor attributes on this node
            var visitorAttr = LoadSymbol(TypeNames.VisitorAttribute);
            var visitorAttrs = node.Symbol.GetAttributes()
                .Where(attr => SymbolEquals(attr.AttributeClass, visitorAttr))
                .ToDictionary(attr => attr.GetCtorValue(0)!.ToString(), attr => (INamedTypeSymbol)attr.GetCtorValue(1)!);
            // Go through all visitors, old or new
            var newVisitors = new Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content, INamedTypeSymbol ReturnType)>();
            foreach (var visitorName in visitors.Keys.Union(visitorAttrs.Select(v => v.Key)))
            {
                if (visitors.TryGetValue(visitorName, out var old) && visitorAttrs.TryGetValue(visitorName, out var newType))
                {
                    // Updated return value
                    newVisitors.Add(visitorName, (old.Root, old.Content, newType));
                }
                else if (visitors.TryGetValue(visitorName, out old))
                {
                    // Just the old value
                    newVisitors.Add(visitorName, (old.Root, old.Content, old.ReturnType));
                }
                else
                {
                    // New thing
                    newVisitors.Add(visitorName, (node.Symbol, new StringBuilder(), visitorAttrs[visitorName]));
                }
            }

            var members = CategorizeAllFields(node.Symbol);
            foreach (var visitor in newVisitors.Values)
            {
                var content = visitor.Content;
                var returnType = visitor.ReturnType.ToDisplayString();
                content.AppendLine($"protected virtual {returnType} Visit({node.Symbol.ToDisplayString()} node) {{");
                // Visit each member of the type
                foreach (var (member, kind) in members)
                {
                    if (kind == FieldKind.Subnode)
                    {
                        // It's a subnode, we have to visit it (with a null check)
                        content.AppendLine($"if (node.{member.Name} != null) this.Visit(node.{member.Name});");
                    }
                    else if (kind == FieldKind.SubnodeList)
                    {
                        // It's a list of visitable things, visit them
                        content.AppendLine($"foreach (var item in node.{member.Name}) this.Visit(item);");
                    }
                }
                // If has subtypes, visit the proper type
                if (node.Children.Count > 0)
                {
                    // Generate visit for concrete type
                    content.AppendLine("switch (node) {");
                    var i = 0;
                    foreach (var child in node.Children.Values)
                    {
                        var subnodeType = child.Symbol.ToDisplayString();
                        content.AppendLine($"case {subnodeType} sub{i}:");
                        content.AppendLine($"    Visit(sub{i});");
                        content.AppendLine("    break;");
                        ++i;
                    }
                    content.AppendLine("}");
                }
                if (returnType != "void") content.AppendLine("    return default;");
                content.AppendLine("}");
            }

            // Unify all results
            var unified = new Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content)>();
            foreach (var cv in newVisitors)
            {
                unified[cv.Key] = (cv.Value.Root, cv.Value.Content);
            }
            foreach (var child in node.Children.Values)
            {
                var childResult = GenerateVisitors(child, newVisitors);
                foreach (var cv in childResult) unified[cv.Key] = cv.Value;
            }
            return unified;
        }

        private (List<string> Equality, List<string> Hash) GenerateEqualityElements(
            ISymbol symbol, 
            List<(IFieldSymbol Symbol, FieldKind Kind)> fields)
        {
            var equality = new List<string>();
            var hash = new List<string>();
            foreach (var field in fields)
            {
                if (field.Kind == FieldKind.LeafList || field.Kind == FieldKind.SubnodeList)
                {
                    // Use .SequenceEquals and add each element to hash one by one
                    equality.Add($"this.{field.Symbol.Name}.Count == other.{field.Symbol.Name}.Count && this.{field.Symbol.Name}.SequenceEqual(other.{field.Symbol.Name})");
                    hash.Add($"foreach (var item in this.{field.Symbol.Name}) hash.Add(item);");
                }
                else
                {
                    // Just call Equals and add to hash
                    equality.Add($"this.{field.Symbol.Name}.Equals(other.{field.Symbol.Name})");
                    hash.Add($"hash.Add(this.{field.Symbol.Name});");
                }
            }
            return (equality, hash);
        }

        private List<(IFieldSymbol Symbol, FieldKind Kind)> CategorizeAllFields(ITypeSymbol symbol) => symbol
            .GetMembers()
            .Where(m => !m.IsStatic)
            .OfType<IFieldSymbol>()
            .Select(f => (Symbol: f, Kind: CategorizeField(f)))
            .ToList();

        private FieldKind CategorizeField(IFieldSymbol symbol)
        {
            var readOnlyList = LoadSymbol(TypeNames.IReadOnlyList);
            if (HasAttribute(symbol.Type, TypeNames.AstAttribute))
            {
                // It's a subnode
                return FieldKind.Subnode;
            }
            else if (symbol.Type.ImplementsGenericInterface(readOnlyList, out var args))
            {
                // It's a list of something
                if (HasAttribute(args![0], TypeNames.AstAttribute))
                {
                    // It's a list of subnodes
                    return FieldKind.SubnodeList;
                }
                else
                {
                    // It's a list of leaves
                    return FieldKind.LeafList;
                }
            }
            else
            {
                // It's some leaf
                return FieldKind.Leaf;
            }
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
