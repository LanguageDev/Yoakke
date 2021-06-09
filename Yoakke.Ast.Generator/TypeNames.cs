namespace Yoakke.Ast.Generator
{
    internal static class TypeNames
    {
        private const string AstNamespace = "Yoakke.Ast";

        public static readonly string AstAttribute = $"{AstNamespace}.Attributes.AstAttribute";
        public static readonly string ImplementEqualityAttribute = $"{AstNamespace}.Attributes.ImplementEqualityAttribute";
        public static readonly string VisitorAttribute = $"{AstNamespace}.Attributes.VisitorAttribute";

        public static readonly string IAstNode = $"{AstNamespace}.IAstNode";
        public static readonly string PrettyPrintFormat = $"{AstNamespace}.PrettyPrintFormat";

        // Printing
        public const string HashCode = "System.HashCode";
        public const string ICloneable = "System.ICloneable";
        public const string IEquatable = "System.IEquatable";
        public const string IEnumerable = "System.Collections.Generic.IEnumerable";
        public const string StringBuilder = "System.Text.StringBuilder";
        public const string KeyValuePair = "System.Collections.Generic.KeyValuePair";
        public const string IReadOnlyCollection = "System.Collections.Generic.IReadOnlyCollection";
        // Loading
        public const string IReadOnlyListG = "System.Collections.Generic.IReadOnlyList`1";
        public const string IReadOnlyCollectionG = "System.Collections.Generic.IReadOnlyCollection`1";
    }
}
