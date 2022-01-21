// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Parser.Generator.Model;

/// <summary>
/// Represents the available token-kinds (terminal kinds) the parser will be able to work with.
/// </summary>
internal class TokenKindSet
{
    /// <summary>
    /// The enum that defines the token-kinds.
    /// </summary>
    public INamedTypeSymbol? EnumType { get; }

    /// <summary>
    /// The fields (token-kinds) defined in the kind-enum <see cref="EnumType"/>.
    /// </summary>
    public IReadOnlyDictionary<string, IFieldSymbol> Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenKindSet"/> class.
    /// </summary>
    /// <param name="enumType">The enum symbol containing the token types.</param>
    public TokenKindSet(INamedTypeSymbol? enumType = null)
    {
        if (enumType is null)
        {
            this.Fields = new Dictionary<string, IFieldSymbol>();
        }
        else
        {
            this.EnumType = enumType;
            // NOTE: False-positive, symbol is not a key-type
#pragma warning disable RS1024 // Compare symbols correctly
            this.Fields = enumType.GetMembers().OfType<IFieldSymbol>().ToDictionary(s => s.Name);
#pragma warning restore RS1024 // Compare symbols correctly
        }
    }

    /// <summary>
    /// Attempts to retrieve a token type variant by name.
    /// </summary>
    /// <param name="name">The name of the variant to retrieve.</param>
    /// <param name="symbol">The fields symbol gets written here, if found.</param>
    /// <returns>True, if a token-type field was found with name <paramref name="name"/>.</returns>
    public bool TryGetVariant(string name, out IFieldSymbol symbol) => this.Fields.TryGetValue(name, out symbol);
}
