// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.TextSynchronization;

/// <summary>
/// Defines how the host (editor) should sync document changes to the language
/// server.
/// </summary>
public enum TextDocumentSyncKind
{
  /// <summary>
  /// Documents should not be synced at all.
  /// </summary>
  None = 0,

  /// <summary>
  /// Documents are synced by always sending the full content
  /// of the document.
  /// </summary>
  Full = 1,

  /// <summary>
  /// Documents are synced by sending the full content on open.
  /// After that only incremental updates to the document are
  /// send.
  /// </summary>
  Incremental = 2,
}
