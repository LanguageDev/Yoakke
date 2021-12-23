// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Server;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#semanticTokensLegend.
/// </summary>
public class SemanticTokensLegend
{
  /// <summary>
  /// The token types a server uses.
  /// </summary>
  [JsonProperty("tokenTypes")]
  public IReadOnlyList<string> TokenTypes { get; set; } = Array.Empty<string>();

  /// <summary>
  /// The token modifiers a server uses.
  /// </summary>
  [JsonProperty("tokenModifiers")]
  public IReadOnlyList<string> TokenModifiers { get; set; } = Array.Empty<string>();
}
