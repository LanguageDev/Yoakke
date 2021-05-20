using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

            if (TryGetAttribute(symbol, TypeNames.ImplementCloneAttribute, out var cloneAttr))
            {
                node.ImplementClone =
                    cloneAttr.ConstructorArguments.Length == 0 || (bool?)cloneAttr.ConstructorArguments[0].Value == true;
            }
            if (TryGetAttribute(symbol, TypeNames.ImplementEqualityAttribute, out var equalityAttr))
            {
                node.ImplementEquality =
                    equalityAttr.ConstructorArguments.Length == 0 || (bool?)equalityAttr.ConstructorArguments[0].Value == true;
            }
            if (TryGetAttribute(symbol, TypeNames.ImplementHashAttribute, out var hashAttr))
            {
                node.ImplementHash =
                    hashAttr.ConstructorArguments.Length == 0 || (bool?)hashAttr.ConstructorArguments[0].Value == true;
            }

            return node;
        }
    }
}
