using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class WorkspaceFoldersServerCapabilities
    {
        /// <summary>
        /// The server has support for workspace folders
        /// </summary>
        [JsonProperty("supported", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Supported { get; set; }

        /// <summary>
        /// Whether the server wants to receive workspace folder
        /// change notifications.
        ///
        /// If a string is provided, the string is treated as an ID
        /// under which the notification is registered on the client
        /// side. The ID can be used to unregister for these events
        /// using the `client/unregisterCapability` request.
        /// </summary>
        [JsonProperty("changeNotifications", NullValueHandling = NullValueHandling.Ignore)]
        public Either<string, bool>? ChangeNotifications { get; set; }
    }
}
