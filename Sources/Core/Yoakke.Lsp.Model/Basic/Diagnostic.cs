// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#diagnostic.
/// </summary>
public class Diagnostic
{
  /// <summary>
  /// The range at which the message applies.
  /// </summary>
  [JsonProperty("range")]
  public Range Range { get; set; }

  /// <summary>
  /// The diagnostic's severity. Can be omitted. If omitted it is up to the
  /// client to interpret diagnostics as error, warning, info or hint.
  /// </summary>
  [JsonProperty("severity", NullValueHandling = NullValueHandling.Ignore)]
  public DiagnosticSeverity? Severity { get; set; }

  /// <summary>
  /// The diagnostic's code, which might appear in the user interface.
  /// </summary>
  [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
  public Either<int, string>? Code { get; set; }

  /// <summary>
  /// An optional property to describe the error code.
  /// </summary>
  [Since(3, 16, 0)]
  [JsonProperty("codeDescription", NullValueHandling = NullValueHandling.Ignore)]
  public CodeDescription? CodeDescription { get; set; }

  /// <summary>
  /// A human-readable string describing the source of this
  /// diagnostic, e.g. 'typescript' or 'super lint'.
  /// </summary>
  [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
  public string? Source { get; set; }

  /// <summary>
  /// The diagnostic's message.
  /// </summary>
  [JsonProperty("message")]
  public string Message { get; set; } = string.Empty;

  /// <summary>
  /// Additional metadata about the diagnostic.
  /// </summary>
  [Since(3, 15, 0)]
  [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
  public IReadOnlyList<DiagnosticTag>? Tags { get; set; }

  /// <summary>
  /// An array of related diagnostic information, e.g. when symbol-names within
  /// a scope collide all definitions can be marked via this property.
  /// </summary>
  [JsonProperty("relatedInformation", NullValueHandling = NullValueHandling.Ignore)]
  public IReadOnlyList<DiagnosticRelatedInformation>? RelatedInformation { get; set; }

  /// <summary>
  /// A data entry field that is preserved between a
  /// `textDocument/publishDiagnostics` notification and
  /// `textDocument/codeAction` request.
  /// </summary>
  [Since(3, 16, 0)]
  [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
  public object? Data { get; set; }
}
