using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ast.Generator
{
    internal static class TypeNames
    {
        private static readonly string AstNamespace = "Yoakke.Ast";

        public static readonly string AstAttribute = $"{AstNamespace}.AstAttribute";
        public static readonly string ImplementCloneAttribute = $"{AstNamespace}.ImplementCloneAttribute";
        public static readonly string ImplementEqualityAttribute = $"{AstNamespace}.ImplementEqualityAttribute";
        public static readonly string ImplementHashAttribute = $"{AstNamespace}.ImplementHashAttribute";
    }
}
