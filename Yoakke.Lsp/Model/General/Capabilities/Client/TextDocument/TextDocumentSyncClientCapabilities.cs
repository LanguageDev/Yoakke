using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class TextDocumentSyncClientCapabilities
    {
        /// <summary>
        /// Whether text document synchronization supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// The client supports sending will save notifications.
        /// </summary>
        [JsonProperty("willSave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSave { get; set; }
        /// <summary>
        /// The client supports sending a will save request and
        /// waits for a response providing text edits which will
        /// be applied to the document before it is saved.
        /// </summary>
        [JsonProperty("willSaveWaitUntil", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WillSaveWaitUntil { get; set; }
        /// <summary>
        /// The client supports did save notifications.
        /// </summary>
        [JsonProperty("didSave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DidSave { get; set; }
    }
}
