// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#documentLinkClientCapabilities.
    /// </summary>
    public class DocumentLinkClientCapabilities
    {
        /// <summary>
        /// Whether document link supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports the `tooltip` property on `DocumentLink`.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("tooltipSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TooltipSupport { get; set; }
    }
}
