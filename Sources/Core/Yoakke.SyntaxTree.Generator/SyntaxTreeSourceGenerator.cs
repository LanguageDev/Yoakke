// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// Generator for syntax tree functionality.
    /// </summary>
    [Generator]
    public class SyntaxTreeSourceGenerator : GeneratorBase
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<TypeDeclarationSyntax> CandidateNodeTypes { get; } = new List<TypeDeclarationSyntax>();

            public IList<TypeDeclarationSyntax> CandidateVisitorTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax)
                {
                    this.CandidateNodeTypes.Add(typeDeclSyntax);
                    if (typeDeclSyntax.AttributeLists.Count > 0) this.CandidateVisitorTypes.Add(typeDeclSyntax);
                }
            }
        }

        private class SyntaxTreeVisitorAttribute
        {
            public INamedTypeSymbol? NodeType { get; set; }

            public INamedTypeSymbol? ReturnType { get; set; }

            public string? GenericReturnType { get; set; }

            public string? MethodName { get; set; }
        }

        private class NameSymbolPairComparer : IEqualityComparer<(string Name, ITypeSymbol Type)>
        {
            public static readonly NameSymbolPairComparer Instance = new();

            public bool Equals((string Name, ITypeSymbol Type) x, (string Name, ITypeSymbol Type) y) =>
                   x.Name == y.Name
                && SymbolEqualityComparer.Default.Equals(x.Type, y.Type);

            public int GetHashCode((string Name, ITypeSymbol Type) obj) =>
                (obj.Name, SymbolEqualityComparer.Default.GetHashCode(obj.Type)).GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxTreeSourceGenerator"/> class.
        /// </summary>
        public SyntaxTreeSourceGenerator()
            : base("Yoakke.SyntaxTree.Generator")
        {
        }

        /// <inheritdoc/>
        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();

        /// <inheritdoc/>
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        /// <inheritdoc/>
        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            var receiver = (SyntaxReceiver)syntaxReceiver;

            this.RequireLibrary("Yoakke.SyntaxTree");

            // We collect out ast nodes
            // NOTE: False positive since 3.3.2 update
#pragma warning disable RS1024 // Compare symbols correctly
            var nodes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            foreach (var syntax in receiver.CandidateNodeTypes)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (!this.IsSyntaxTreeNode(symbol!)) continue;
                if (!this.RequireDeclarableInside(symbol!)) continue;
                nodes.Add(symbol!);
            }

            // Now generate each node source
            foreach (var node in nodes) this.GenerateNodeSource(node);

            // Now we build visitor descriptions for classes annotated with visitor attributes
            var visitors = new List<Visitor>();
            var visitorAttr = this.LoadSymbol(TypeNames.SyntaxTreeVisitorAttribute);
            foreach (var syntax in receiver.CandidateVisitorTypes)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                var attrs = symbol!.GetAttributes<SyntaxTreeVisitorAttribute>(visitorAttr);
                if (attrs.Count == 0) continue;
                if (!this.RequireDeclarableInside(symbol!)) continue;

                // Construct the visitor
                var visitor = new Visitor(symbol!);
                foreach (var attr in attrs)
                {
                    if (attr.ReturnType is not null && attr.GenericReturnType is not null)
                    {
                        this.Report(Diagnostics.BothReturnTypeAndGenericReturnTypeSpecified, syntax.GetLocation());
                        continue;
                    }

                    visitor.Overrides[attr.NodeType!] = new(attr.NodeType!)
                    {
                        MethodName = attr.MethodName,
                        ReturnType = (object?)attr.ReturnType ?? attr.GenericReturnType,
                    };
                }
                visitors.Add(visitor);
            }

            // We recollect all nodes the visitors reference, as those can be in different assemblies
            var visitableNodes = this.CollectVisitorReferencedNodes(visitors);

            // For the visitors we build a node hierarchy
            var rootNodes = BuildNodeHierarchy(visitableNodes);
            var allNodes = CollectAllMetaNodes(rootNodes);
            // We fill up the hierarchy with visitor information
            foreach (var visitor in visitors)
            {
                foreach (var rootNode in rootNodes) this.PopulateNodeWithVisitorOverrides(visitor, null, rootNode);
            }
            // Now we generate the source for each visitor
            foreach (var visitor in visitors) this.GenerateVisitorSource(visitor, rootNodes, allNodes);
        }

        private void GenerateNodeSource(INamedTypeSymbol nodeSymbol)
        {
            // Generate this node
            var fields = this.GetAllSyntaxTreeNodeProperties(nodeSymbol);
            var definitions = new StringBuilder();
            var interfaceImplementations = new StringBuilder();
            var isRoot = nodeSymbol.BaseType is null || !this.IsSyntaxTreeNode(nodeSymbol.BaseType);

            if (isRoot) interfaceImplementations.Append(" : ").Append(TypeNames.ISyntaxTreeNode);

            // Generate the ISyntaxTreeNode implementation
            var overridePart = isRoot ? string.Empty : "override";
            if (nodeSymbol.IsAbstract)
            {
                definitions.AppendLine($"public abstract {overridePart} int ChildCount {{ get; }}");
                definitions.AppendLine($"public abstract {overridePart} object GetChild(int index);");
            }
            else
            {
                definitions.AppendLine($"public {overridePart} int ChildCount => {fields.Count};");
                definitions
                    .AppendLine($"public {overridePart} object GetChild(int index) => index switch")
                    .AppendLine("{");
                for (var i = 0; i < fields.Count; ++i) definitions.AppendLine($"{i} => this.{fields[i].Name},");
                definitions
                    .AppendLine($"_ => throw new {TypeNames.ArgumentOutOfRangeException}(nameof(index)),")
                    .AppendLine("};");
            }

            // Surrounding crud
            var (prefix, suffix) = nodeSymbol.ContainingSymbol.DeclareInsideExternally();
            var sourceCode = $@"
{prefix}
    partial {nodeSymbol.GetTypeKindName()} {nodeSymbol.Name} {interfaceImplementations} {{
        {definitions}
    }}
{suffix}
";
            this.AddSource($"{nodeSymbol.ToDisplayString()}.Generated.cs", sourceCode);
        }

        private void GenerateVisitorSource(
            Visitor visitor,
            List<MetaNode> rootNodes,
            Dictionary<INamedTypeSymbol, MetaNode> allNodes)
        {
            var voidType = this.LoadSymbol(TypeNames.Void);
            var definitions = new StringBuilder();

            // A lookup table for user-defined single-parameter methods in the visitor class
            // Creates a set of (name, type), so we can easily filter out methods that are defined by the user,
            // so we don't duplicate them
            var userDefinedVisitors = visitor.VisitorClass
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.Parameters.Length == 1)
                .Select(m => (m.Name, m.Parameters[0].Type))
                .ToHashSet(NameSymbolPairComparer.Instance);

            ITypeSymbol GetGenericParam(string name)
            {
                foreach (var param in visitor.VisitorClass.TypeParameters)
                {
                    if (param.Name == name) return param;
                }

                // Not found, error
                this.Report(Diagnostics.GenericTypeNotDefined, visitor.VisitorClass.Locations[0], visitor.VisitorClass.Name, name);
                return voidType;
            }

            void GenerateMembersVisitor(MetaNode currentNode)
            {
                var enumerableInterface = this.LoadSymbol(TypeNames.IEnumerable);
                var members = this.GetAllSyntaxTreeNodeProperties(currentNode.NodeClass);
                foreach (var member in members)
                {
                    var type = member switch
                    {
                        IFieldSymbol f => f.Type,
                        IPropertySymbol p => p.Type,
                        _ => throw new InvalidOperationException(),
                    };
                    if (type.ImplementsGenericInterface(enumerableInterface, out var args)
                        && args![0] is INamedTypeSymbol sym
                        && this.IsSyntaxTreeNode(sym))
                    {
                        // It's a list of visitable things, visit them
                        var child = allNodes[sym];
                        var methodName = child.VisitorOverrides[visitor].MethodName ?? "Visit";
                        definitions!.AppendLine($"foreach (var item in node.{member.Name}) this.{methodName}(item);");
                    }
                    else if (type is INamedTypeSymbol sym2 && this.IsSyntaxTreeNode(sym2))
                    {
                        // It's a subnode, we have to visit it (with a null check)
                        var child = allNodes[sym2];
                        var methodName = child.VisitorOverrides[visitor].MethodName ?? "Visit";
                        definitions!.AppendLine($"if (node.{member.Name} is not null) this.{methodName}(node.{member.Name});");
                    }
                }
            }

            void GenerateNodeVisitor(MetaNode currentNode)
            {
                // Visit children
                foreach (var child in currentNode.Children) GenerateNodeVisitor(child);

                // If the current node has no associated info with this node, we are done
                if (!currentNode.VisitorOverrides.TryGetValue(visitor, out var overrides)) return;

                // Otherwise we need to generate code, as the visitor deals with this node
                var returnType = overrides.ReturnType switch
                {
                    INamedTypeSymbol t => t,
                    string genericParamName => GetGenericParam(genericParamName),
                    null => voidType,
                    _ => throw new InvalidOperationException(),
                };
                var returnTypeStr = returnType.ToDisplayString();
                var methodName = overrides.MethodName ?? "Visit";

                // Here we can check, if the method is already defined by the user and early-terminate, if so
                if (userDefinedVisitors.Contains((methodName, currentNode.NodeClass))) return;

                definitions.AppendLine($"protected virtual {returnTypeStr} {methodName}({currentNode.NodeClass.ToDisplayString()} node) {{");

                if (currentNode.Children.Count == 0)
                {
                    // No children, just visit the proper type members
                    GenerateMembersVisitor(currentNode);

                    // If doesn't return void, provide default
                    if (!SymbolEqualityComparer.Default.Equals(returnType, voidType)) definitions.AppendLine("return default;");
                }
                else
                {
                    // Has children, visit subtypes
                    definitions.AppendLine("switch (node) {");

                    var i = 0;
                    foreach (var child in currentNode.Children)
                    {
                        var subnodeType = child.NodeClass.ToDisplayString();
                        definitions.AppendLine($"case {subnodeType} sub{i}: {{");
                        var subMethodName = child.VisitorOverrides[visitor].MethodName ?? "Visit";
                        if (SymbolEqualityComparer.Default.Equals(returnType, voidType))
                        {
                            definitions.AppendLine($"this.{subMethodName}(sub{i});");
                            definitions.AppendLine("break;");
                        }
                        else
                        {
                            definitions.AppendLine($"return this.{subMethodName}(sub{i});");
                        }
                        definitions.AppendLine("}");
                        ++i;
                    }

                    // We generate a default case
                    definitions.AppendLine("default: {");
                    if (currentNode.NodeClass.IsAbstract)
                    {
                        // We didn't know its subtype, throw
                        definitions.AppendLine($"throw new {TypeNames.NotSupportedException}();");
                    }
                    else
                    {
                        // No subtype, just visit members
                        GenerateMembersVisitor(currentNode);

                        // If returns void, just break, otherwise return default
                        if (SymbolEqualityComparer.Default.Equals(returnType, voidType)) definitions.AppendLine("break;");
                        else definitions.AppendLine("return default;");
                    }

                    definitions.AppendLine("} }");
                }

                definitions.AppendLine("}");
            }

            // Just go through all roots
            foreach (var node in rootNodes) GenerateNodeVisitor(node);

            // Surrounding crud
            var (prefix, suffix) = visitor.VisitorClass.DeclareInsideExternally();
            var sourceCode = $@"
{prefix}
    {definitions}
{suffix}
";
            this.AddSource($"{visitor.VisitorClass.ToDisplayString()}.Generated.cs", sourceCode);
        }

        private void PopulateNodeWithVisitorOverrides(Visitor visitor, VisitorOverride? currentOverride, MetaNode currentNode)
        {
            var voidType = this.LoadSymbol(TypeNames.Void);

            // Override the visitor defaults, if any is provided
            if (visitor.Overrides.TryGetValue(currentNode.NodeClass, out var newOverride))
            {
                // We override by filling in anything new that has been provided
                currentOverride = new(currentNode.NodeClass)
                {
                    MethodName = newOverride.MethodName ?? currentOverride?.MethodName ?? "Visit",
                    ReturnType = newOverride.ReturnType ?? currentOverride?.ReturnType ?? voidType,
                };
            }

            // Assign the current override to the node, if there is any
            if (currentOverride is not null) currentNode.VisitorOverrides[visitor] = currentOverride;

            // Visit children
            foreach (var child in currentNode.Children) this.PopulateNodeWithVisitorOverrides(visitor, currentOverride, child);
        }

        private HashSet<INamedTypeSymbol> CollectVisitorReferencedNodes(List<Visitor> visitors)
        {
            // NOTE: False positive
#pragma warning disable RS1024 // Compare symbols correctly
            var visitableNodes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var visitedAssemblys = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            foreach (var visitor in visitors)
            {
                foreach (var nodeClass in visitor.Overrides.Values.Select(o => o.NodeClass))
                {
                    // If the assembly of the node is already visited, skip the recursive check
                    if (!visitedAssemblys.Add(nodeClass.ContainingAssembly)) continue;

                    // New, unvisited assembly, filter out relevant nodes
                    var typesInAssembly = nodeClass.ContainingAssembly.GetAllDeclaredTypes();
                    foreach (var symbol in typesInAssembly)
                    {
                        if (!this.IsSyntaxTreeNode(symbol)) continue;
                        // NOTE: Here we don't require it to be declarable inside, since the visitor is completely external
                        visitableNodes.Add(symbol);
                    }
                }
            }
            return visitableNodes;
        }

        private static Dictionary<INamedTypeSymbol, MetaNode> CollectAllMetaNodes(List<MetaNode> rootNodes)
        {
            // NOTE: False-positive
#pragma warning disable RS1024 // Compare symbols correctly
            var result = new Dictionary<INamedTypeSymbol, MetaNode>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

            void CollectAllMetaNodesRec(MetaNode current)
            {
                result.Add(current.NodeClass, current);
                foreach (var node in current.Children) CollectAllMetaNodesRec(node);
            }

            foreach (var node in rootNodes) CollectAllMetaNodesRec(node);

            return result;
        }

        private static List<MetaNode> BuildNodeHierarchy(HashSet<INamedTypeSymbol> symbols)
        {
            // First we extract root elements
            var rootSymbols = symbols
                .Where(sym => sym.BaseType is null || !symbols.Contains(sym.BaseType))
                .ToHashSet(SymbolEqualityComparer.Default);
            var rootNodes = rootSymbols
                .Cast<INamedTypeSymbol>()
                .Select(sym => new MetaNode(sym))
                .ToList();

            // Now we loopty-loop and keep growing the tree until we have attached all our nodes into the tree
            var foundNodes = rootNodes.ToDictionary(m => m.NodeClass, SymbolEqualityComparer.Default);
            var remainingNodes = symbols.Except(rootSymbols).ToHashSet(SymbolEqualityComparer.Default);
            while (remainingNodes.Count > 0)
            {
                var toRemove = new List<INamedTypeSymbol>();
                foreach (var symbol in remainingNodes.Cast<INamedTypeSymbol>())
                {
                    if (!foundNodes.TryGetValue(symbol.BaseType, out var parentNode)) continue;
                    // We found this node's parent in our existing nodes
                    var node = new MetaNode(symbol);
                    parentNode.Children.Add(node);
                    foundNodes.Add(symbol, node);
                    toRemove.Add(symbol);
                }

                foreach (var symbol in toRemove) remainingNodes.Remove(symbol);

                // TODO: Maybe error out, we couldn't collect any more nodes
                if (toRemove.Count == 0) break;
            }

            return rootNodes!;
        }

        private List<ISymbol> GetAllSyntaxTreeNodeProperties(ITypeSymbol symbol)
        {
            var ignoreAttr = this.LoadSymbol(TypeNames.SyntaxTreeIgnoreAttribute);
            return symbol
                .GetMembers()
                .Where(m => !m.IsStatic
                         && m.DeclaredAccessibility != Accessibility.Private
                         && !m.HasAttribute(ignoreAttr)
                         && (m is IFieldSymbol || (m is IPropertySymbol p && p.HasBackingField())))
                .ToList();
        }

        private bool IsSyntaxTreeNode(INamedTypeSymbol symbol)
        {
            var ignoreAttr = this.LoadSymbol(TypeNames.SyntaxTreeIgnoreAttribute);
            var treeAttr = this.LoadSymbol(TypeNames.SyntaxTreeAttribute);

            if (symbol.HasAttribute(ignoreAttr)) return false;
            if (symbol.HasAttribute(treeAttr)) return true;
            if (symbol.BaseType is null) return false;
            return this.IsSyntaxTreeNode(symbol.BaseType);
        }
    }
}
