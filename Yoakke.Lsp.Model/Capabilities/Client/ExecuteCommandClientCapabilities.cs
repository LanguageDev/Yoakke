using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    public class ExecuteCommandClientCapabilities
    {
        /// <summary>
        /// Execute command supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
