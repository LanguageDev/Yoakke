using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class TextDocumentItem
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }
        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        [JsonProperty("languageId")]
        public string LanguageId { get; set; }
        /// <summary>
        /// The version number of this document (it will increase after each
        /// change, including undo/redo).
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
