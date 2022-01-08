// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    /// <summary>
    /// Client capabilities specific to the used markdown parser.
    /// </summary>
    [Since(3, 16, 0)]
    public class MarkdownClientCapabilities
    {
        /// <summary>
        /// The name of the parser.
        /// </summary>
        [JsonProperty("parser")]
        public string Parser { get; set; } = string.Empty;

        /// <summary>
        /// The version of the parser.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
    }
}