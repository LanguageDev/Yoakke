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
            public IList<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax)
                {
                    this.CandidateTypes.Add(typeDeclSyntax);
                }
            }
        }

        private class SyntaxTreeVisitorAttribute
        {
            public string ClassName { get; set; } = string.Empty;

            public INamedTypeSymbol? ReturnType { get; set; }
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

            this.RequireLibrary("Yoakke.SyntaxTree");

            this.BuildMetaNodes(receiver.CandidateTypes);

            // Now generate each node source
            foreach (var node in this.allNodes.Values) this.GenerateNodeSource(node);

            // Generate visitor contents
            foreach (var node in this.rootNodes.Values) this.GenerateVisitorSource(node);
        }

        private void BuildMetaNodes(IList<TypeDeclarationSyntax> classDeclarations)
        {
            // Collect only the classes that have the AstAttribute or inherit from a class that has AstAttribute
            // NOTE: False positive since 3.3.2 update
#pragma warning disable RS1024 // Compare symbols correctly
            var astNodeSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            foreach (var syntax in classDeclarations)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (!this.IsSyntaxTreeNode(symbol!)) continue;
                if (!this.RequirePartial(syntax)) continue;
                astNodeSymbols.Add(symbol!);
            }

            // Select all the root nodes
            // They are all the nodes without a base class or with a base class that's not an AST node
            var rootNodeSymbols = astNodeSymbols
                .OfType<INamedTypeSymbol>()
                .Where(sym => sym.BaseType is null || !astNodeSymbols.Contains(sym.BaseType))
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
            var visitors = this.GenerateVisitors(node, new Dictionary<string, Visitor>());
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
    partial {visitor.Value.Owner.Symbol.GetTypeKindName()} {visitor.Value.Owner.Name} {{
        public abstract class {visitor.Key} {{
            {visitor.Value.Code}
        }}
    }}
    {nestClassSuffix}
}}
";
                this.AddSource($"{node.Symbol.ToDisplayString()}.{visitor.Key}.Generated.cs", sourceCode);
            }
        }

        private Dictionary<string, Visitor> GenerateVisitors(MetaNode node, IReadOnlyDictionary<string, Visitor> visitors)
        {
            // Get all visitor attributes on this node
            var visitorOverrides = this.GetVisitors(node).ToDictionary(n => n.Name);
            // Go through all visitors, old or new, keep the newer definition
            var newVisitors = new Dictionary<string, Visitor>();
            foreach (var name in visitors.Keys.Union(visitorOverrides.Keys))
            {
                var oldHas = visitors.TryGetValue(name, out var oldDef);
                var newHas = visitorOverrides.TryGetValue(name, out var newDef);
                if (oldHas && newHas)
                {
                    // Return-type override
                    newVisitors.Add(name, new(oldDef.Owner, name, newDef.ReturnType) { Code = oldDef.Code });
                }
                else if (newHas)
                {
                    // Completely new
                    newVisitors.Add(name, newDef);
                }
                else
                {
                    // Old
                    newVisitors.Add(name, oldDef);
                }
            }

            var members = this.GetAllSyntaxTreeNodeChildren(node.Symbol);
            var enumerableInterface = this.LoadSymbol(TypeNames.IEnumerable);
            foreach (var visitor in newVisitors.Values)
            {
                var returnTypeStr = visitor.ReturnType.ToDisplayString();
                visitor.Code
                    .AppendLine($"protected virtual {returnTypeStr} Visit({node.Symbol.ToDisplayString()} node)")
                    .AppendLine("{");
                if (node.Children.Count == 0)
                {
                    // No subtypes
                    // Visit each member of the type
                    this.GenerateMemberVisitors(visitor, members);
                }
                else if (node.Children.Count > 0)
                {
                    // If has subtypes, visit the proper type
                    // Generate visit for concrete type
                    visitor.Code.AppendLine("switch (node) {");
                    var i = 0;
                    foreach (var child in node.Children.Values)
                    {
                        var subnodeType = child.Symbol.ToDisplayString();
                        visitor.Code
                            .AppendLine($"case {subnodeType} sub{i}:")
                            .AppendLine("{");
                        if (returnTypeStr == "void")
                        {
                            visitor.Code.AppendLine($"    this.Visit(sub{i});");
                            visitor.Code.AppendLine("    break;");
                        }
                        else
                        {
                            visitor.Code.AppendLine($"    return this.Visit(sub{i});");
                        }
                        visitor.Code.AppendLine("}");
                        ++i;
                    }
                    // Default case
                    visitor.Code
                        .AppendLine("default:")
                        .AppendLine("{");
                    if (node.IsAbstract)
                    {
                        visitor.Code.AppendLine($"    throw new {TypeNames.NotSupportedException}();");
                    }
                    else
                    {
                        // No a subtype
                        // Visit each member of the type
                        this.GenerateMemberVisitors(visitor, members);
                    }
                    visitor.Code
                        .AppendLine("    break;")
                        .AppendLine("}")
                        .AppendLine("}");
                }
                if (returnTypeStr != "void") visitor.Code.AppendLine("    return default;");
                visitor.Code.AppendLine("}");
            }

            // Unify all results
            var unified = newVisitors.Values.ToDictionary(v => v.Name);
            foreach (var child in node.Children.Values)
            {
                var childResult = this.GenerateVisitors(child, newVisitors);
                foreach (var cv in childResult) unified[cv.Key] = cv.Value;
            }
            return unified;
        }

        private void GenerateMemberVisitors(Visitor visitor, IReadOnlyList<ISymbol> members)
        {
            var enumerableInterface = this.LoadSymbol(TypeNames.IEnumerable);
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
                    visitor.Code.AppendLine($"foreach (var item in node.{member.Name}) this.Visit(item);");
                }
                else if (type is INamedTypeSymbol sym2 && this.IsSyntaxTreeNode(sym2))
                {
                    // It's a subnode, we have to visit it (with a null check)
                    visitor.Code.AppendLine($"if (node.{member.Name} is not null) this.Visit(node.{member.Name});");
                }
            }
        }

        private List<ISymbol> GetAllSyntaxTreeNodeChildren(ITypeSymbol symbol)
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

        private IReadOnlyList<Visitor> GetVisitors(MetaNode node)
        {
            var result = new List<Visitor>();
            var visitorAttr = this.LoadSymbol(TypeNames.SyntaxTreeVisitorAttribute);
            var voidType = this.LoadSymbol(TypeNames.Void);
            var visitorAttrs = node.Symbol.GetAttributes<SyntaxTreeVisitorAttribute>(visitorAttr);
            foreach (var attr in visitorAttrs)
            {
                var returnType = attr.ReturnType ?? voidType;
                result.Add(new(node, attr.ClassName, returnType));
            }
            return result;
        }
    }
}
