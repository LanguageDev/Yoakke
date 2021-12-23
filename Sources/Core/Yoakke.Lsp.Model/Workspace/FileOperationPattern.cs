// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Workspace;

/// <summary>
/// A pattern to describe in which file operation requests or notifications
/// the server is interested in.
/// </summary>
[Since(3, 16, 0)]
public class FileOperationPattern
{
  /// <summary>
  /// The glob pattern to match. Glob patterns can have the following syntax:
  /// - `*` to match one or more characters in a path segment
  /// - `?` to match on one character in a path segment
  /// - `**` to match any number of path segments, including none
  /// - `{}` to group sub patterns into an OR expression. (e.g. `**?/*.{ts,js}`
  /// matches all TypeScript and JavaScript files)
  /// - `[]` to declare a range of characters to match in a path segment
  /// (e.g., `example.[0-9]` to match on `example.0`, `example.1`, .)
  /// - `[!...]` to negate a range of characters to match in a path segment
  /// (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
  /// not `example.0`).
  /// </summary>
  [JsonProperty("glob")]
  public string Glob { get; set; } = string.Empty;

  /// <summary>
  /// Whether to match files or folders with this pattern.
  ///
  /// Matches both if undefined.
  /// </summary>
  [JsonProperty("matches", NullValueHandling = NullValueHandling.Ignore)]
  public FileOperationPatternKind? Matches { get; set; }

  /// <summary>
  /// Additional options used during matching.
  /// </summary>
  [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
  public FileOperationPatternOptions? Options { get; set; }
}
