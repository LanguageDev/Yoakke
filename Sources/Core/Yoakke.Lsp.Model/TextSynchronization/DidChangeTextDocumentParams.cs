// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#didChangeTextDocumentParams.
    /// </summary>
    public class DidChangeTextDocumentParams
    {
        /// <summary>
        /// The document that did change. The version number points
        /// to the version after all provided content changes have
        /// been applied.
        /// </summary>
        [JsonProperty("textDocument")]
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The actual content changes. The content changes describe single state
        /// changes to the document. So if there are two content changes c1 (at
        /// array index 0) and c2 (at array index 1) for a document in state S then
        /// c1 moves the document from S to S' and c2 from S' to S''. So c1 is
        /// computed on the state S and c2 is computed on the state S'.
        ///
        /// To mirror the content of a document using change events use the following
        /// approach:
        /// - start with the same initial content
        /// - apply the 'textDocument/didChange' notifications in the order you
        /// receive them.
        /// - apply the `TextDocumentContentChangeEvent`s in a single notification
        /// in the order you receive them.
        /// </summary>
        [JsonProperty("contentChanges")]
        public IReadOnlyList<TextDocumentContentChangeEvent> ContentChanges { get; set; }
    }
}
