using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class SemanticTokensRegistrationOptions : SemanticTokensOptions, ITextDocumentRegistrationOptions, IStaticRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }
    }
}
