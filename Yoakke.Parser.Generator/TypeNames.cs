using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal static class TypeNames
    {
        private static readonly string ParserNamespace = "Yoakke.Parser";

        public static readonly string ParserAttribute = $"{ParserNamespace}.ParserAttribute";
        public static readonly string RuleAttribute = $"{ParserNamespace}.RuleAttribute";
    }
}
