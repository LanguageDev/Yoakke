// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// The https://microsoft.github.io/language-server-protocol/specifications/specification-3-17/#location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The <see cref="DocumentUri"/> for the file.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// The <see cref="Basic.Range"/> in the file.
        /// </summary>
        [JsonProperty("range")]
        public Range Range { get; set; }
    }
}
