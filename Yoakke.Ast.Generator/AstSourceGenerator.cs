using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            // Collect only the classes that have the AstAttribute
            var astNodeClasses = receiver.CandidateClasses
                .Select(syntax =>
                {
                    var model = Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                    return model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                })
                .Where(symbol => HasAttribute(symbol!, TypeNames.AstAttribute))
                .ToList();

            // Select all the root nodes
            // They are all the nodes without a base class or with a base class that's not an AST node
            rootNodes = astNodeClasses
                .Where(sym => sym!.BaseType != null && !HasAttribute(sym.BaseType, TypeNames.AstAttribute))
                .Select(sym => MakeMetaNode(sym!, null))
                .ToDictionary(n => n.Name);
        }

        private MetaNode MakeMetaNode(INamedTypeSymbol symbol, MetaNode? parent)
        {
            var node = new MetaNode(symbol, parent);
            if (parent != null) parent.Children.Add(node);

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
