// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;

namespace Yoakke.SyntaxTree.Generator;

/// <summary>
/// Generator for syntax tree functionality.
/// </summary>
[Generator]
public class SyntaxTreeSourceGenerator : GeneratorBase
{
    private class SyntaxReceiver : ISyntaxReceiver
    {
        public IList<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

        public IList<TypeDeclarationSyntax> CandidateVisitorTypes { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax)
            {
                this.CandidateTypes.Add(typeDeclSyntax);
            }
        }
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
        foreach (var syntax in receiver.CandidateTypes)
        {
            var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
            if (!this.IsSyntaxTreeNode(symbol!)) continue;
            if (!this.RequireDeclarableInside(symbol!)) continue;
            nodes.Add(symbol!);
        }

        // Now generate each node source
        foreach (var node in nodes) this.GenerateNodeSource(node);
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
