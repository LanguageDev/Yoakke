// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document.
        ///
        /// The version number of a document will increase after each change,
        /// including undo/redo. The number doesn't need to be consecutive.
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
