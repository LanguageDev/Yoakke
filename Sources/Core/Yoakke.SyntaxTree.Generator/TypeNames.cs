// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// Common type-name constants.
    /// </summary>
    internal static class TypeNames
    {
        private const string AstNamespace = "Yoakke.SyntaxTree";

        /// <summary>
        /// System.ArgumentOutOfRangeException.
        /// </summary>
        public const string ArgumentOutOfRangeException = "System.ArgumentOutOfRangeException";

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
        /// System.InvalidOperationException.
        /// </summary>
        public const string InvalidOperationException = "System.InvalidOperationException";

        /// <summary>
        /// System.Collections.Generic.IEnumerable.
        /// </summary>
        public const string IEnumerable = "System.Collections.Generic.IEnumerable`1";

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
        /// System.Void.
        /// </summary>
        public const string Void = "System.Void";

        /// <summary>
        /// The attribute to annotate a syntax tree.
        /// </summary>
        public static readonly string SyntaxTreeAttribute = $"{AstNamespace}.Attributes.SyntaxTreeAttribute";

        /// <summary>
        /// The attribute to annotate if a visitor should be implemented.
        /// </summary>
        public static readonly string SyntaxTreeVisitorAttribute = $"{AstNamespace}.Attributes.SyntaxTreeVisitorAttribute";

        /// <summary>
        /// The attribute to mark a member to be skipped.
        /// </summary>
        public static readonly string SyntaxTreeIgnoreAttribute = $"{AstNamespace}.Attributes.SyntaxTreeIgnoreAttribute";

        /// <summary>
        /// The common interface for AST nodes.
        /// </summary>
        public static readonly string ISyntaxTreeNode = $"{AstNamespace}.ISyntaxTreeNode";
    }
}
