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
            public IList<TypeDeclarationSyntax> CandidateClasses { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax
                    && typeDeclSyntax.AttributeLists.Count > 0)
                {
                    this.CandidateClasses.Add(typeDeclSyntax);
                }
            }
        }

        private Dictionary<string, MetaNode> rootNodes = new();
        private Dictionary<string, MetaNode> allNodes = new();

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
            // Debugger.Launch();

            this.RequireLibrary("Yoakke.SyntaxTree");

            this.BuildMetaNodes(receiver.CandidateClasses);

            // Now generate each node source
            foreach (var node in this.allNodes.Values) this.GenerateNodeSource(node);

            // Generate visitor contents
            foreach (var node in this.rootNodes.Values) this.GenerateVisitorSource(node);
        }

        private void BuildMetaNodes(IList<TypeDeclarationSyntax> classDeclarations)
        {
            // Collect only the classes that have the AstAttribute
            // NOTE: False positive since 3.3.2 update
#pragma warning disable RS1024 // Compare symbols correctly
            var astNodeSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            foreach (var syntax in classDeclarations)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (!this.HasAttribute(symbol!, TypeNames.SyntaxTreeAttribute)) continue;
                if (!this.RequirePartial(syntax)) continue;
                astNodeSymbols.Add(symbol!);
            }

            // Select all the root nodes
            // They are all the nodes without a base class or with a base class that's not an AST node
            var rootNodeSymbols = astNodeSymbols
                .OfType<INamedTypeSymbol>()
                .Where(sym => sym.BaseType is null || !this.HasAttribute(sym.BaseType, TypeNames.SyntaxTreeAttribute))
                .ToHashSet(SymbolEqualityComparer.Default);
            this.rootNodes = rootNodeSymbols
                .OfType<INamedTypeSymbol>()
                .Select(sym => new MetaNode(sym, null))
                .ToDictionary(n => n.Name);
            // Clone them to all nodes
            this.allNodes = this.rootNodes.ToDictionary(n => n.Key, n => n.Value);

            // Remove them from all symbols
            astNodeSymbols = astNodeSymbols
                .Except(rootNodeSymbols)
                .ToHashSet(SymbolEqualityComparer.Default)!;

            // Now loop until all nodes could be attached somewhere
            while (astNodeSymbols.Count > 0)
            {
                // NOTE: False positive since 3.3.2 update
#pragma warning disable RS1024 // Compare symbols correctly
                var toRemove = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
                foreach (var symbol in astNodeSymbols.OfType<INamedTypeSymbol>())
                {
                    if (!this.allNodes.TryGetValue(symbol.BaseType!.Name, out var parentNode)) continue;
                    // We found this node's parent in our existing nodes
                    var node = new MetaNode(symbol, parentNode);
                    this.allNodes.Add(node.Name, node);
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

        private void GenerateNodeSource(MetaNode node)
        {
            // Generate this node
            var fields = this.GetAllSyntaxTreeNodeChildren(node.Symbol);
            var definitions = new StringBuilder();
            var interfaceImplementations = new StringBuilder();

            if (node.Parent is null) interfaceImplementations.Append(" : ").Append(TypeNames.ISyntaxTreeNode);

            // Generate the ISyntaxTreeNode implementation
            var overridePart = node.Parent is null ? string.Empty : "override";
            if (node.IsAbstract)
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
            var surroundingNamespace = node.Symbol.ContainingNamespace.ToDisplayString();
            var nestClassPrefix = new StringBuilder();
            var nestClassSuffix = new StringBuilder();
            foreach (var nest in node.Nesting)
            {
                nestClassPrefix.AppendLine($"partial {nest} {{");
                nestClassSuffix.AppendLine("}");
            }

            var sourceCode = $@"
namespace {surroundingNamespace} {{
    {nestClassPrefix}
    partial {node.Symbol.GetTypeKindName()} {node.Name} {interfaceImplementations} {{
        {definitions}
    }}
    {nestClassSuffix}
}}
";
            this.AddSource($"{node.Symbol.ToDisplayString()}.Generated.cs", sourceCode);
        }

        private void GenerateVisitorSource(MetaNode node)
        {
            var visitors = this.GenerateVisitors(node, new Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content, INamedTypeSymbol ReturnType)>());
            foreach (var visitor in visitors)
            {
                // Surrounding crud
                var surroundingNamespace = node.Symbol.ContainingNamespace.ToDisplayString();
                var nestClassPrefix = new StringBuilder();
                var nestClassSuffix = new StringBuilder();
                foreach (var nest in node.Nesting)
                {
                    nestClassPrefix.AppendLine($"partial {nest} {{");
                    nestClassSuffix.AppendLine("}");
                }

                var sourceCode = $@"
namespace {surroundingNamespace} {{
    {nestClassPrefix}
    partial {visitor.Value.Root.GetTypeKindName()} {visitor.Value.Root.Name} {{
        public abstract class {visitor.Key} {{
            {visitor.Value.Content}
        }}
    }}
    {nestClassSuffix}
}}
";
                this.AddSource($"{node.Symbol.ToDisplayString()}.{visitor.Key}.Generated.cs", sourceCode);
            }
        }

        private Dictionary<string, (INamedTypeSymbol Root, StringBuilder Content)> GenerateVisitors(
            MetaNode node,
            IReadOnlyDictionary<string, (INamedTypeSymbol Root, StringBuilder Content, INamedTypeSymbol ReturnType)> visitors)
        {
            // Get all visitor attributes on this node
            var visitorAttr = this.LoadSymbol(TypeNames.SyntaxTreeVisitorAttribute);
            var visitorAttrs = node.Symbol.GetAttributes()
                .Where(attr => SymbolEquals(attr.AttributeClass, visitorAttr))
                .ToDictionary(
                    attr => attr.GetCtorValue(0)!.ToString(),
                    attr => attr.ConstructorArguments.Length > 1 ? (INamedTypeSymbol)attr.GetCtorValue(1)! : this.LoadSymbol(TypeNames.Void));
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

            var members = this.GetAllSyntaxTreeNodeChildren(node.Symbol);
            var enumerableInterface = this.LoadSymbol(TypeNames.IEnumerable);
            foreach (var (root, content, returnType) in newVisitors.Values)
            {
                var returnTypeStr = returnType.ToDisplayString();
                content.AppendLine($"protected virtual {returnTypeStr} Visit({node.Symbol.ToDisplayString()} node) {{");
                if (node.Children.Count == 0)
                {
                    // No subtypes
                    // Visit each member of the type
                    foreach (var member in members)
                    {
                        var type = member switch
                        {
                            IFieldSymbol f => f.Type,
                            IPropertySymbol p => p.Type,
                            _ => throw new InvalidOperationException(),
                        };
                        if (type.ImplementsGenericInterface(enumerableInterface, out var args)
                         && this.HasAttribute(args![0], TypeNames.SyntaxTreeAttribute))
                        {
                            // It's a list of visitable things, visit them
                            content.AppendLine($"foreach (var item in node.{member.Name}) this.Visit(item);");
                        }
                        else if (this.HasAttribute(type, TypeNames.SyntaxTreeAttribute))
                        {
                            // It's a subnode, we have to visit it (with a null check)
                            content.AppendLine($"if (node.{member.Name} is not null) this.Visit(node.{member.Name});");
                        }
                    }
                }
                else if (node.Children.Count > 0)
                {
                    // TODO: Visit fields if non-abstract?
                    // TODO: error in default case if abstract?
                    // If has subtypes, visit the proper type
                    // Generate visit for concrete type
                    content.AppendLine("switch (node) {");
                    var i = 0;
                    foreach (var child in node.Children.Values)
                    {
                        var subnodeType = child.Symbol.ToDisplayString();
                        content.AppendLine($"case {subnodeType} sub{i}:");
                        if (returnTypeStr == "void")
                        {
                            content.AppendLine($"    Visit(sub{i});");
                            content.AppendLine("    break;");
                        }
                        else
                        {
                            content.AppendLine($"    return Visit(sub{i});");
                        }
                        ++i;
                    }
                    content.AppendLine("}");
                }
                if (returnTypeStr != "void") content.AppendLine("    return default;");
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
                var childResult = this.GenerateVisitors(child, newVisitors);
                foreach (var cv in childResult) unified[cv.Key] = cv.Value;
            }
            return unified;
        }

        private List<ISymbol> GetAllSyntaxTreeNodeChildren(ITypeSymbol symbol) => symbol
            .GetMembers()
            .Where(m => !m.IsStatic
                     && m.DeclaredAccessibility != Accessibility.Private
                     && !this.HasAttribute(m, TypeNames.SyntaxTreeIgnoreAttribute)
                     && (m is IFieldSymbol || m is IPropertySymbol))
            .ToList();
    }
}
