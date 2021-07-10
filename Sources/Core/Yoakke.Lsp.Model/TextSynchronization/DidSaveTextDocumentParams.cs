// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#didSaveTextDocumentParams.
    /// </summary>
    public class DidSaveTextDocumentParams
    {
        /// <summary>
        /// The document that was saved.
        /// </summary>
        [JsonProperty("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// Optional the content when saved. Depends on the includeText value
        /// when the save notification was requested.
        /// </summary>
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; set; }
    }
}
