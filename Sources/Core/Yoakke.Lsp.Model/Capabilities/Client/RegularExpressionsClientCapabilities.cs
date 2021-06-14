// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    /// <summary>
    /// Client capabilities specific to regular expressions.
    /// </summary>
    public class RegularExpressionsClientCapabilities
    {
        /// <summary>
        /// The engine's name.
        /// </summary>
        [JsonProperty("engine")]
        public string Engine { get; set; }

        /// <summary>
        /// The engine's version.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
    }
}
