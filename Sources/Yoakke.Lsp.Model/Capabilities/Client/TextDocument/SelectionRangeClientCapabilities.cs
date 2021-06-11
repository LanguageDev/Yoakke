// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class SelectionRangeClientCapabilities
    {
        /// <summary>
        /// Whether implementation supports dynamic registration for selection range
        /// providers. If this is set to `true` the client supports the new
        /// `SelectionRangeRegistrationOptions` return value for the corresponding
        /// server capability as well.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
