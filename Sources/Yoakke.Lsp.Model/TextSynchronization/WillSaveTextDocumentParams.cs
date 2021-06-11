// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// The parameters send in a will save text document notification.
    /// </summary>
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        /// The document that will be saved.
        /// </summary>
        [JsonProperty("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The 'TextDocumentSaveReason'.
        /// </summary>
        [JsonProperty("reason")]
        public TextDocumentSaveReason Reason { get; set; }
    }
}
