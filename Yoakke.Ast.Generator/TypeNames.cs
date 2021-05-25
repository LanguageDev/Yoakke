using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ast.Generator
{
    internal static class TypeNames
    {
        private static readonly string AstNamespace = "Yoakke.Ast";

        public static readonly string AstAttribute = $"{AstNamespace}.Attributes.AstAttribute";
        public static readonly string ImplementEqualityAttribute = $"{AstNamespace}.Attributes.ImplementEqualityAttribute";
        public static readonly string VisitorAttribute = $"{AstNamespace}.Attributes.VisitorAttribute";

        public static readonly string IAstNode = $"{AstNamespace}.IAstNode";
        public static readonly string PrettyPrintFormat = $"{AstNamespace}.PrettyPrintFormat";

        public static readonly string HashCode = "System.HashCode";
        public static readonly string ICloneable = "System.ICloneable";
        public static readonly string IEquatable = "System.IEquatable";
        public static readonly string IReadOnlyList = "System.Collections.Generic.IReadOnlyList`1";
        public static readonly string IEnumerable = "System.Collections.Generic.IEnumerable";
        public static readonly string StringBuilder = $"System.Text.StringBuilder";
    }
}
