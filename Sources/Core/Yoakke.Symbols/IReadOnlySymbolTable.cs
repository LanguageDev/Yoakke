// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Symbols;

/// <summary>
/// A symbol table that contains scopes and symbol associations.
/// </summary>
public interface IReadOnlySymbolTable
{
  /// <summary>
  /// The global scope in the symbol hierarchy.
  /// </summary>
  public IReadOnlyScope GlobalScope { get; }
}
