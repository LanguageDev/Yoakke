using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }
    }
}
