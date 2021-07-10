// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#didOpenTextDocumentParams.
    /// </summary>
    public class DidOpenTextDocumentParams
    {
        /// <summary>
        /// The document that was opened.
        /// </summary>
        [JsonProperty("textDocument")]
        public TextDocumentItem TextDocument { get; set; }
    }
}
