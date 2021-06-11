// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions
{
    public class TextDocumentSaveRegistrationOptions : ITextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }

        /// <summary>
        /// The client is supposed to include the content on save.
        /// </summary>
        [JsonProperty("includeText", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeText { get; set; }
    }
}
