using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class Range
    {
        /// <summary>
        /// The range's start position.
        /// </summary>
        [JsonProperty("start")]
        public Position Start { get; set; }

        /// <summary>
        /// The range's end position.
        /// </summary>
        [JsonProperty("end")]
        public Position End { get; set; }
    }
}
