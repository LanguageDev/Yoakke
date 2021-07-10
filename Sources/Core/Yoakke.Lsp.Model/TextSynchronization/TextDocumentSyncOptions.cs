// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocumentSyncOptions.
    /// </summary>
    public class TextDocumentSyncOptions
    {
        /// <summary>
        /// Open and close notifications are sent to the server. If omitted open
        /// close notification should not be sent.
        /// </summary>
        [JsonProperty("openClose", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OpenClose { get; set; }

        /// <summary>
        /// Change notifications are sent to the server. See
        /// TextDocumentSyncKind.None, TextDocumentSyncKind.Full and
        /// TextDocumentSyncKind.Incremental. If omitted it defaults to
        /// TextDocumentSyncKind.None.
        /// </summary>
        [JsonProperty("change", NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentSyncKind? Change { get; set; }

        /// <summary>
        /// If present will save notifications are sent to the server. If omitted
        /// the notification should not be sent.
        /// </summary>
        [JsonProperty("willSave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSave { get; set; }

        /// <summary>
        /// If present will save wait until requests are sent to the server. If
        /// omitted the request should not be sent.
        /// </summary>
        [JsonProperty("willSaveWaitUntil", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSaveWaitUntil { get; set; }

        /// <summary>
        /// If present save notifications are sent to the server. If omitted the
        /// notification should not be sent.
        /// </summary>
        [JsonProperty("save", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, SaveOptions>? Save { get; set; }
    }
}
