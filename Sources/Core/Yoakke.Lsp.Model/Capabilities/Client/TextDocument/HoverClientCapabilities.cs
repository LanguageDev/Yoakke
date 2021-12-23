// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocument_hover.
/// </summary>
public class HoverClientCapabilities
{
  /// <summary>
  /// Whether hover supports dynamic registration.
  /// </summary>
  [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
  public bool? DynamicRegistration { get; set; }

  /// <summary>
  /// Client supports the follow content formats if the content
  /// property refers to a `literal of type MarkupContent`.
  /// The order describes the preferred format of the client.
  /// </summary>
  [JsonProperty("contentFormat", NullValueHandling = NullValueHandling.Ignore)]
  public IReadOnlyList<MarkupKind>? ContentFormat { get; set; }
}
