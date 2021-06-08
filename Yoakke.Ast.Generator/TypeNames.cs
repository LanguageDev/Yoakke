namespace Yoakke.Ast.Generator
{
    internal static class TypeNames
    {
        private const string AstNamespace = "Yoakke.Ast";

        private static readonly string AstAttribute = $"{AstNamespace}.Attributes.AstAttribute";
        private static readonly string ImplementEqualityAttribute = $"{AstNamespace}.Attributes.ImplementEqualityAttribute";
        private static readonly string VisitorAttribute = $"{AstNamespace}.Attributes.VisitorAttribute";

        private static readonly string IAstNode = $"{AstNamespace}.IAstNode";
        private static readonly string PrettyPrintFormat = $"{AstNamespace}.PrettyPrintFormat";

        // Printing
        private const string HashCode = "System.HashCode";
        private const string ICloneable = "System.ICloneable";
        private const string IEquatable = "System.IEquatable";
        private const string IEnumerable = "System.Collections.Generic.IEnumerable";
        private const string StringBuilder = "System.Text.StringBuilder";
        private const string KeyValuePair = "System.Collections.Generic.KeyValuePair";
        private const string IReadOnlyCollection = "System.Collections.Generic.IReadOnlyCollection";
        // Loading
        private const string IReadOnlyListG = "System.Collections.Generic.IReadOnlyList`1";
        private const string IReadOnlyCollectionG = "System.Collections.Generic.IReadOnlyCollection`1";
    }
}
