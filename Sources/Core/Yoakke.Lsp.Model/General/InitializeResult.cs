// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Capabilities.Server;

namespace Yoakke.Lsp.Model.General;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#initializeResult.
/// </summary>
public class InitializeResult
{
  /// <summary>
  /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#initializeResult.
  /// </summary>
  public class ServerInformation
  {
    /// <summary>
    /// The name of the server as defined by the server.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The server's version as defined by the server.
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public string? Version { get; set; }
  }

  /// <summary>
  /// The capabilities the language server provides.
  /// </summary>
  [JsonProperty("capabilities")]
  public ServerCapabilities Capabilities { get; set; } = new();

  /// <summary>
  /// Information about the server.
  /// </summary>
  [Since(3, 15, 0)]
  [JsonProperty("serverInfo", NullValueHandling = NullValueHandling.Ignore)]
  public ServerInformation? ServerInfo { get; set; }
}
