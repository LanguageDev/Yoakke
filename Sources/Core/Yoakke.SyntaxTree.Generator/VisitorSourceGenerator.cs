// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// Generator for visitor functionality.
    /// </summary>
    [Generator]
    public class VisitorSourceGenerator : GeneratorBase
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax && typeDeclSyntax.AttributeLists.Count > 0)
                {
                    this.CandidateTypes.Add(typeDeclSyntax);
                }
            }
        }

        private class VisitorAttribute
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
        /// Initializes a new instance of the <see cref="VisitorSourceGenerator"/> class.
        /// </summary>
        public VisitorSourceGenerator()
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

            // Now we build visitor descriptions for classes annotated with visitor attributes
            var visitors = new List<Visitor>();
            var visitorAttr = this.LoadSymbol(TypeNames.VisitorAttribute);
            foreach (var syntax in receiver.CandidateTypes)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                var attrs = symbol!.GetAttributes<VisitorAttribute>(visitorAttr);
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

            // We process each visitor separately
            foreach (var visitor in visitors)
            {
                // Collect all nodes relevant to this visitor
                var allNodes = this.CollectAllNodes(visitor);
                // Collect out root nodes
                var rootNodes = allNodes.Values.Where(n => n.Parent is null).ToList();
                // We fill up the hierarchy with visitor information
                foreach (var rootNode in rootNodes) this.PopulateNodeWithVisitorOverrides(visitor, null, rootNode);
                // Now we generate the source for the visitor
                this.GenerateVisitorSource(visitor, rootNodes, allNodes);
            }
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
                var members = this.GetAllNodeProperties(currentNode.NodeClass);
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
                        && allNodes.TryGetValue(sym, out var child))
                    {
                        // It's a list of visitable things, visit them
                        var methodName = child.VisitorOverrides[visitor].MethodName ?? "Visit";
                        definitions!.AppendLine($"foreach (var item in node.{member.Name}) this.{methodName}(item);");
                    }
                    else if (type is INamedTypeSymbol sym2 && allNodes.TryGetValue(sym2, out child))
                    {
                        // It's a subnode, we have to visit it (with a null check)
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

        private Dictionary<INamedTypeSymbol, MetaNode> CollectAllNodes(Visitor visitor)
        {
            // NOTE: False positive
#pragma warning disable RS1024 // Compare symbols correctly
            var visitableNodes = new Dictionary<INamedTypeSymbol, MetaNode>(SymbolEqualityComparer.Default);
            var visitedAssemblys = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly

            // Before we can do anything, we have to register the initial overrides
            foreach (var @override in visitor.Overrides.Values)
            {
                var node = new MetaNode(@override.NodeClass);
                visitableNodes[node.NodeClass] = node;
                if (node.NodeClass.BaseType is not null
                 && visitableNodes.TryGetValue(node.NodeClass.BaseType, out var parent))
                {
                    // We found the parent here, connect them
                    parent.AddChild(node);
                }
            }

            // Collect all subtypes
            foreach (var nodeClass in visitor.Overrides.Values.Select(o => o.NodeClass))
            {
                // If the assembly of the node is already visited, skip the recursive check
                if (!visitedAssemblys.Add(nodeClass.ContainingAssembly)) continue;

                // New, unvisited assembly, filter out relevant nodes
                var typesInAssembly = nodeClass.ContainingAssembly.GetAllDeclaredTypes();
                foreach (var symbol in typesInAssembly)
                {
                    if (!this.IsVisitableNode(visitableNodes, symbol)) continue;
                    // NOTE: Here we don't require it to be declarable inside, since the visitor is completely external
                }
            }
            return visitableNodes;
        }

        private bool IsVisitableNode(Dictionary<INamedTypeSymbol, MetaNode> allNodes, INamedTypeSymbol symbol)
        {
            var ignoreAttr = this.LoadSymbol(TypeNames.SyntaxTreeIgnoreAttribute);

            if (allNodes.ContainsKey(symbol)) return true;
            if (symbol.HasAttribute(ignoreAttr)) return false;
            if (symbol.BaseType is null) return false;
            if (allNodes.TryGetValue(symbol.BaseType, out var parent))
            {
                var child = new MetaNode(symbol);
                parent.AddChild(child);
                allNodes[symbol] = child;
                return true;
            }
            if (this.IsVisitableNode(allNodes, symbol.BaseType))
            {
                parent = allNodes[symbol.BaseType];
                var child = new MetaNode(symbol);
                parent.AddChild(child);
                allNodes[symbol] = child;
                return true;
            }
            return false;
        }

        private List<ISymbol> GetAllNodeProperties(ITypeSymbol symbol)
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
    }
}
