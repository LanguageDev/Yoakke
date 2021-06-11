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

        public TokenKindSet()
        {
            this.Fields = new Dictionary<string, IFieldSymbol>();
        }

        public TokenKindSet(INamedTypeSymbol enumType)
        {
            this.EnumType = enumType;
            this.Fields = enumType.GetMembers().OfType<IFieldSymbol>().ToDictionary(s => s.Name);
        }

        public bool TryGetVariant(string name, out IFieldSymbol symbol) => this.Fields.TryGetValue(name, out symbol);
    }
}
