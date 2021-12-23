// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Workspace;

/// <summary>
/// A filter to describe in which file operation requests or notifications
/// the server is interested in.
/// </summary>
[Since(3, 16, 0)]
public class FileOperationFilter
{
  /// <summary>
  /// A Uri like `file` or `untitled`.
  /// </summary>
  [JsonProperty("scheme", NullValueHandling = NullValueHandling.Ignore)]
  public string? Scheme { get; set; }

  /// <summary>
  /// The actual file operation pattern.
  /// </summary>
  [JsonProperty("pattern")]
  public FileOperationPattern Pattern { get; set; } = new();
}
