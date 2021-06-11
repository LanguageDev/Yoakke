using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Capabilities.Client;
using Yoakke.Lsp.Model.Workspace;

namespace Yoakke.Lsp.Model.General
{
    public class InitializeParams : IWorkDoneProgressParams
    {
        public class ClientInformation
        {
            /// <summary>
            /// The name of the client as defined by the client.
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// The client's version as defined by the client.
            /// </summary>
            [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
            public string? Version { get; set; }
        }

        /// <summary>
        /// An optional token that a server can use to report work done progress.
        /// </summary>
        [JsonProperty("workDoneToken", NullValueHandling = NullValueHandling.Ignore)]
        public ProgressToken? WorkDoneToken { get; set; }

        /// <summary>
        /// The process Id of the parent process that started the server. Is null if
        /// the process has not been started by another process. If the parent
        /// process is not alive then the server should exit (see exit notification)
        /// its process.
        /// </summary>
        [JsonProperty("processId")]
        public int? ProcessId { get; set; }

        /// <summary>
        /// Information about the client
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("clientInfo", NullValueHandling = NullValueHandling.Ignore)]
        public ClientInformation? ClientInfo { get; set; }

        /// <summary>
        /// The locale the client is currently showing the user interface
        /// in. This must not necessarily be the locale of the operating
        /// system.
        ///
        /// Uses IETF language tags as the value's syntax
        /// (See https://en.wikipedia.org/wiki/IETF_language_tag)
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
        public string? Locale { get; set; }

        /// <summary>
        /// The rootPath of the workspace. Is null
        /// if no folder is open.
        /// </summary>
        [Obsolete("in favour of `rootUri`.")]
        [JsonProperty("rootPath", NullValueHandling = NullValueHandling.Ignore)]
        public string? RootPath { get; set; }

        /// <summary>
        /// The rootUri of the workspace. Is null if no
        /// folder is open. If both `rootPath` and `rootUri` are set
        /// `rootUri` wins.
        /// </summary>
        [Obsolete("in favour of `workspaceFolders`")]
        [JsonProperty("rootUri")]
        public DocumentUri? RootUri { get; set; }

        /// <summary>
        /// User provided initialization options.
        /// </summary>
        [JsonProperty("initializationOptions", NullValueHandling = NullValueHandling.Ignore)]
        public object? InitializationOptions { get; set; }

        /// <summary>
        /// The capabilities provided by the client (editor or tool)
        /// </summary>
        [JsonProperty("capabilities")]
        public ClientCapabilities Capabilities { get; set; }

        /// <summary>
        /// The initial trace setting. If omitted trace is disabled ('off').
        /// </summary>
        [JsonProperty("trace", NullValueHandling = NullValueHandling.Ignore)]
        public TraceValue? Trace { get; set; }

        /// <summary>
        /// The workspace folders configured in the client when the server starts.
        /// This property is only available if the client supports workspace folders.
        /// It can be `null` if the client supports workspace folders but none are
        /// configured.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("workspaceFolders", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<WorkspaceFolder>? WorkspaceFolders { get; set; }
    }
}
