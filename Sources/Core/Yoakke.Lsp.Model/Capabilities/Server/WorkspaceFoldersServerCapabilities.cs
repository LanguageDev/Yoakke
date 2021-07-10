// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workspaceFoldersServerCapabilities.
    /// </summary>
    public class WorkspaceFoldersServerCapabilities
    {
        /// <summary>
        /// The server has support for workspace folders
        /// </summary>
        [JsonProperty("supported", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Supported { get; set; }

        /// <summary>
        /// Whether the server wants to receive workspace folder
        /// change notifications.
        ///
        /// If a string is provided, the string is treated as an ID
        /// under which the notification is registered on the client
        /// side. The ID can be used to unregister for these events
        /// using the `client/unregisterCapability` request.
        /// </summary>
        [JsonProperty("changeNotifications", NullValueHandling = NullValueHandling.Ignore)]
        public Either<string, bool>? ChangeNotifications { get; set; }
    }
}
