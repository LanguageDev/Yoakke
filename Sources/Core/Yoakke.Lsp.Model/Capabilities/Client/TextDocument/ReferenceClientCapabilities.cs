// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#referenceClientCapabilities.
    /// </summary>
    public class ReferenceClientCapabilities
    {
        /// <summary>
        /// Whether references supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}