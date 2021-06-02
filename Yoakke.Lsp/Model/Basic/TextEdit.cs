using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert
        /// text into a document create a range where start === end.
        /// </summary>
        [JsonProperty("range")]
        public Range Range { get; set; }
        /// <summary>
        /// The string to be inserted. For delete operations use an
        /// empty string.
        /// </summary>
        [JsonProperty("newText")]
        public string NewText { get; set; }
    }
}
