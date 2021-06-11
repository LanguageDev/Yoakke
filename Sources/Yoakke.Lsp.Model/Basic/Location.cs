// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class Location
    {
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }

        [JsonProperty("range")]
        public Range Range { get; set; }
    }
}
