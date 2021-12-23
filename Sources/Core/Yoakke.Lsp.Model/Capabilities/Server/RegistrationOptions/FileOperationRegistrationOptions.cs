// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Workspace;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions;

/// <summary>
/// The options to register for file operations.
/// </summary>
[Since(3, 16, 0)]
public class FileOperationRegistrationOptions
{
  /// <summary>
  /// The actual filters.
  /// </summary>
  [JsonProperty("filters")]
  public IReadOnlyList<FileOperationFilter> Filters { get; set; } = Array.Empty<FileOperationFilter>();
}
