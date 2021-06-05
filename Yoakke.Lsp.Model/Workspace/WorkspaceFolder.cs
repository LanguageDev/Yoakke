using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Workspace
{
    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }
        /// <summary>
        /// The name of the workspace folder. Used to refer to this
        /// workspace folder in the user interface.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
