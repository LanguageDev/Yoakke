using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client
{
    public class DidChangeWatchedFilesClientCapabilities
    {
        /// <summary>
        /// Did change watched files notification supports dynamic registration.
        /// Please note that the current protocol doesn't support static
        /// configuration for file changes from the server side.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
