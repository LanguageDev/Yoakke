using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    public class SaveOptions
    {
        /// <summary>
        /// The client is supposed to include the content on save.
        /// </summary>
        [JsonProperty("includeText", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeText { get; set; }
    }
}
