// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentSyncClientCapabilities.
    /// </summary>
    public class TextDocumentSyncClientCapabilities
    {
        /// <summary>
        /// Whether text document synchronization supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        [JsonProperty("willSave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSave { get; set; }

        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        [JsonProperty("willSaveWaitUntil", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSaveWaitUntil { get; set; }

        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        [JsonProperty("didSave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DidSave { get; set; }
    }
}
