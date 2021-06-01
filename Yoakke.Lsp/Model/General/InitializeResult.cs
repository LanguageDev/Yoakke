using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.General
{
    public class InitializeResult
    {
        public class ServerInformation
        {
            /// <summary>
            /// The name of the server as defined by the server.
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }
            /// <summary>
            /// The server's version as defined by the server.
            /// </summary>
            [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
            public string? Version { get; set; }
        }

        /// <summary>
        /// The capabilities the language server provides.
        /// </summary>
        //[JsonProperty("capabilities")]
        //public ServerCapabilities Capabilities { get; set; }
        /// <summary>
        /// Information about the server.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("serverInfo", NullValueHandling = NullValueHandling.Ignore)]
        public ServerInformation? ServerInfo { get; set; }
    }
}
