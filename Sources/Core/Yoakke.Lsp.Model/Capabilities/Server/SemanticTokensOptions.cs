// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#semanticTokensOptions.
/// </summary>
public class SemanticTokensOptions : IWorkDoneProgressOptions
{
  /// <summary>
  /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#semanticTokensOptions.
  /// </summary>
  public class RangeOptions
  {
  }

  /// <summary>
  /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#semanticTokensOptions.
  /// </summary>
  public class FullOptions
  {
    /// <summary>
    /// The server supports deltas for full documents.
    /// </summary>
    [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Delta { get; set; }
  }

  /// <inheritdoc/>
  [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
  public bool? WorkDoneProgress { get; set; }

  /// <summary>
  /// The legend used by the server.
  /// </summary>
  [JsonProperty("legend")]
  public SemanticTokensLegend Legend { get; set; } = new();

  /// <summary>
  /// Server supports providing semantic tokens for a specific range
  /// of a document.
  /// </summary>
  [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
  public Either<bool, RangeOptions>? Range { get; set; }

  /// <summary>
  /// Server supports providing semantic tokens for a full document.
  /// </summary>
  [JsonProperty("full", NullValueHandling = NullValueHandling.Ignore)]
  public Either<bool, FullOptions>? Full { get; set; }
}
