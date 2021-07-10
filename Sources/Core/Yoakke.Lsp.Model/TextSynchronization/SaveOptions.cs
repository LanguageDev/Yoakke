// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#saveOptions.
    /// </summary>
    public class SaveOptions
    {
        /// <summary>
        /// The client is supposed to include the content on save.
        /// </summary>
        [JsonProperty("includeText", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeText { get; set; }
    }
}
