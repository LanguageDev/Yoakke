using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal class TokenKindSet
    {
        public readonly INamedTypeSymbol EnumType;
        private Dictionary<string, IFieldSymbol> fields;

        public TokenKindSet()
        {
            fields = new Dictionary<string, IFieldSymbol>();
        }

        public TokenKindSet(INamedTypeSymbol enumType)
        {
            EnumType = enumType;
            fields = enumType.GetMembers().OfType<IFieldSymbol>().ToDictionary(s => s.Name);
        }

        public bool TryGetVariant(string name, out IFieldSymbol symbol) => fields.TryGetValue(name, out symbol);
    }
}
