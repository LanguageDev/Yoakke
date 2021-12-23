// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client;

/// <summary>
/// Client capabilities for the show document request.
/// </summary>
[Since(3, 16, 0)]
public class ShowDocumentClientCapabilities
{
  /// <summary>
  /// The client has support for the show document
  /// request.
  /// </summary>
  [JsonProperty("support")]
  public bool Support { get; set; }
}
