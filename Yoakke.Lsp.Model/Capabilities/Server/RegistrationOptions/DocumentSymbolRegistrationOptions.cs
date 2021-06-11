﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions
{
    public class DocumentSymbolRegistrationOptions : DocumentSymbolOptions, ITextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }
    }
}
