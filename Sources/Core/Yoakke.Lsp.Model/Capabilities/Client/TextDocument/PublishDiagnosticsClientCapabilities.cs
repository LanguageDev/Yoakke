// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#publishDiagnosticsClientCapabilities.
/// </summary>
public class PublishDiagnosticsClientCapabilities
{
  /// <summary>
  /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#publishDiagnosticsClientCapabilities.
  /// </summary>
  public class TagSupportCapabilities
  {
    /// <summary>
    /// The tags supported by the client.
    /// </summary>
    [JsonProperty("valueSet")]
    public IReadOnlyList<DiagnosticTag> ValueSet { get; set; } = Array.Empty<DiagnosticTag>();
  }

  /// <summary>
  /// Whether the clients accepts diagnostics with related information.
  /// </summary>
  [JsonProperty("relatedInformation", NullValueHandling = NullValueHandling.Ignore)]
  public bool? RelatedInformation { get; set; }

  /// <summary>
  /// Client supports the tag property to provide meta data about a diagnostic.
  /// Clients supporting tags have to handle unknown tags gracefully.
  /// </summary>
  [Since(3, 15, 0)]
  [JsonProperty("tagSupport", NullValueHandling = NullValueHandling.Ignore)]
  public TagSupportCapabilities? TagSupport { get; set; }

  /// <summary>
  /// Whether the client interprets the version property of the
  /// `textDocument/publishDiagnostics` notification's parameter.
  /// </summary>
  [Since(3, 15, 0)]
  [JsonProperty("versionSupport", NullValueHandling = NullValueHandling.Ignore)]
  public bool? VersionSupport { get; set; }

  /// <summary>
  /// Client supports a codeDescription property.
  /// </summary>
  [Since(3, 16, 0)]
  [JsonProperty("codeDescriptionSupport", NullValueHandling = NullValueHandling.Ignore)]
  public bool? CodeDescriptionSupport { get; set; }

  /// <summary>
  /// Whether code action supports the `data` property which is
  /// preserved between a `textDocument/publishDiagnostics` and
  /// `textDocument/codeAction` request.
  /// </summary>
  [Since(3, 16, 0)]
  [JsonProperty("dataSupport", NullValueHandling = NullValueHandling.Ignore)]
  public bool? DataSupport { get; set; }
}
