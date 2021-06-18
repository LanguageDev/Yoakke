// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// Structure to capture a description for an error code.
    /// </summary>
    [Since(3, 16, 0)]
    public class CodeDescription
    {
        /// <summary>
        /// An URI to open with more information about the diagnostic error.
        /// </summary>
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }
}
