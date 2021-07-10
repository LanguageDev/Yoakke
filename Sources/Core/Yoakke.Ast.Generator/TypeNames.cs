// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Ast.Generator
{
    /// <summary>
    /// Common type-name constants.
    /// </summary>
    internal static class TypeNames
    {
        private const string AstNamespace = "Yoakke.Ast";

        /// <summary>
        /// System.HashCode.
        /// </summary>
        public const string HashCode = "System.HashCode";

        /// <summary>
        /// System.ICloneable.
        /// </summary>
        public const string ICloneable = "System.ICloneable";

        /// <summary>
        /// System.IEquatable.
        /// </summary>
        public const string IEquatable = "System.IEquatable";

        /// <summary>
        /// System.Collections.Generic.IEnumerable.
        /// </summary>
        public const string IEnumerable = "System.Collections.Generic.IEnumerable";

        /// <summary>
        /// System.Text.StringBuilder.
        /// </summary>
        public const string StringBuilder = "System.Text.StringBuilder";

        /// <summary>
        /// System.Collections.Generic.KeyValuePair.
        /// </summary>
        public const string KeyValuePair = "System.Collections.Generic.KeyValuePair";

        /// <summary>
        /// System.Collections.Generic.IReadOnlyCollection.
        /// </summary>
        public const string IReadOnlyCollection = "System.Collections.Generic.IReadOnlyCollection";

        /// <summary>
        /// System.Collections.Generic.IReadOnlyList`1.
        /// </summary>
        public const string IReadOnlyListG = "System.Collections.Generic.IReadOnlyList`1";

        /// <summary>
        /// System.Collections.Generic.IReadOnlyCollection`1.
        /// </summary>
        public const string IReadOnlyCollectionG = "System.Collections.Generic.IReadOnlyCollection`1";

        /// <summary>
        /// The attribute to annotate an AST.
        /// </summary>
        public static readonly string AstAttribute = $"{AstNamespace}.Attributes.AstAttribute";

        /// <summary>
        /// The attribute to annotate if equality and hash should be implemented.
        /// </summary>
        public static readonly string ImplementEqualityAttribute = $"{AstNamespace}.Attributes.ImplementEqualityAttribute";

        /// <summary>
        /// The attribute to annotate if a visitor should be implemented.
        /// </summary>
        public static readonly string VisitorAttribute = $"{AstNamespace}.Attributes.VisitorAttribute";

        /// <summary>
        /// The common interface for AST nodes.
        /// </summary>
        public static readonly string IAstNode = $"{AstNamespace}.IAstNode";

        /// <summary>
        /// The enumeration containing pretty-print formats.
        /// </summary>
        public static readonly string PrettyPrintFormat = $"{AstNamespace}.PrettyPrintFormat";
    }
}
