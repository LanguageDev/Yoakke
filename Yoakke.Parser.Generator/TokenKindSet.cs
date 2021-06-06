using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// Represents the available token-kinds (terminal kinds) the parser will be able to work with.
    /// </summary>
    internal class TokenKindSet
    {
        /// <summary>
        /// The enum that defines the token-kinds.
        /// </summary>
        public readonly INamedTypeSymbol? EnumType;

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
