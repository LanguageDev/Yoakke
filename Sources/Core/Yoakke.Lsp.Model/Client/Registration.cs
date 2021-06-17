// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Client
{
    /// <summary>
    /// General parameters to register for a capability.
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister
        /// the request again.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The method / capability to register for.
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Options necessary for the registration.
        /// </summary>
        [JsonProperty("registerOptions", NullValueHandling = NullValueHandling.Ignore)]
        public object? RegisterOptions { get; set; }
    }
}
