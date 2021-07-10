// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentItem.
    /// </summary>
    public class TextDocumentItem
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        [JsonProperty("languageId")]
        public string LanguageId { get; set; } = string.Empty;

        /// <summary>
        /// The version number of this document (it will increase after each
        /// change, including undo/redo).
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
