// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#didChangeConfigurationClientCapabilities.
    /// </summary>
    public class DidChangeConfigurationClientCapabilities
    {
        /// <summary>
        /// Did change configuration notification supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
