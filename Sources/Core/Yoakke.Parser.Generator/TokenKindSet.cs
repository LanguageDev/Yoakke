// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenKindSet"/> class.
        /// </summary>
        public TokenKindSet()
        {
            this.Fields = new Dictionary<string, IFieldSymbol>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenKindSet"/> class.
        /// </summary>
        /// <param name="enumType">The enum symbol containing the token types.</param>
        public TokenKindSet(INamedTypeSymbol enumType)
        {
            this.EnumType = enumType;
            this.Fields = enumType.GetMembers().OfType<IFieldSymbol>().ToDictionary(s => s.Name);
        }

        /// <summary>
        /// Attempts to retrieve a token type variant by name.
        /// </summary>
        /// <param name="name">The name of the variant to retrieve.</param>
        /// <param name="symbol">The fields symbol gets written here, if found.</param>
        /// <returns>True, if a token-type field was found with name <paramref name="name"/>.</returns>
        public bool TryGetVariant(string name, out IFieldSymbol symbol) => this.Fields.TryGetValue(name, out symbol);
    }
}
