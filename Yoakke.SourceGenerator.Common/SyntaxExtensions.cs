using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SourceGenerator.Common
{
    public static class SyntaxExtensions
    {
        public static bool IsPartial(this TypeDeclarationSyntax syntax) =>
            syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
}
