// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#documentRangeFormattingOptions.
/// </summary>
public class DocumentRangeFormattingOptions : IWorkDoneProgressOptions
{
  /// <inheritdoc/>
  [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
  public bool? WorkDoneProgress { get; set; }
}
