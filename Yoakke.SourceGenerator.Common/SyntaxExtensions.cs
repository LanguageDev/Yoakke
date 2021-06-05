using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Yoakke.SourceGenerator.Common
{
    public static class SyntaxExtensions
    {
        public static bool IsPartial(this TypeDeclarationSyntax syntax) =>
            syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
}
