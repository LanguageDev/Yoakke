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
