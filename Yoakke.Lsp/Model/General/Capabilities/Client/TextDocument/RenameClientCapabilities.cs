using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class RenameClientCapabilities
    {
        /// <summary>
        /// Whether rename supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// Client supports testing for validity of rename operations
        /// before execution.
        /// </summary>
        [Since(3, 12, 0)]
        [JsonProperty("prepareSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PrepareSupport { get; set; }
        /// <summary>
        /// Client supports the default behavior result
        /// (`{ defaultBehavior: boolean }`).
        ///
        /// The value indicates the default behavior used by the
        /// client.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("prepareSupportDefaultBehavior", NullValueHandling = NullValueHandling.Ignore)]
        public PrepareSupportDefaultBehavior? PrepareSupportDefaultBehavior { get; set; }
        /// <summary>
        /// Whether th client honors the change annotations in
        /// text edits and resource operations returned via the
        /// rename request's workspace edit by for example presenting
        /// the workspace edit in the user interface and asking
        /// for confirmation.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("honorsChangeAnnotations", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HonorsChangeAnnotations { get; set; }
    }
}
