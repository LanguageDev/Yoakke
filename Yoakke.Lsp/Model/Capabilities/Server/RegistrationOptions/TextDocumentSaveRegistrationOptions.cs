using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
