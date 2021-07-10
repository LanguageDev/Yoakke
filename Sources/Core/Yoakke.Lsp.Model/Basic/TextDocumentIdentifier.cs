// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentIdentifier.
    /// </summary>
    public class TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }
    }
}
