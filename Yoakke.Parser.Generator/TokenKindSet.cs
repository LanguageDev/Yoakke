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
        /// <summary>
        /// The fields (token-kinds) defined in the kind-enum <see cref="EnumType"/>.
        /// </summary>
        public readonly IReadOnlyDictionary<string, IFieldSymbol> Fields;

        public TokenKindSet()
        {
            Fields = new Dictionary<string, IFieldSymbol>();
        }

        public TokenKindSet(INamedTypeSymbol enumType)
        {
            EnumType = enumType;
            Fields = enumType.GetMembers().OfType<IFieldSymbol>().ToDictionary(s => s.Name);
        }

        public bool TryGetVariant(string name, out IFieldSymbol symbol) => Fields.TryGetValue(name, out symbol);
    }
}
