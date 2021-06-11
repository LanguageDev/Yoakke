using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Yoakke.SourceGenerator.Common
{
    public static class SyntaxExtensions
    {
        public static bool IsPartial(this TypeDeclarationSyntax syntax) =>
            syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
}
