using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Capabilities.Client.TextDocument;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    public class ClientCapabilities
    {
        public class WorkspaceCapabilities
        {
            public class FileOperationsCapabilities
            {
                /// <summary>
                /// Whether the client supports dynamic registration for file
                /// requests/notifications.
                /// </summary>
                [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
                public bool? DynamicRegistration { get; set; }

                /// <summary>
                /// The client has support for sending didCreateFiles notifications.
                /// </summary>
                [JsonProperty("didCreate", NullValueHandling = NullValueHandling.Ignore)]
                public bool? DidCreate { get; set; }

                /// <summary>
                /// The client has support for sending willCreateFiles requests.
                /// </summary>
                [JsonProperty("willCreate", NullValueHandling = NullValueHandling.Ignore)]
                public bool? WillCreate { get; set; }

                /// <summary>
                /// The client has support for sending didRenameFiles notifications.
                /// </summary>
                [JsonProperty("didRename", NullValueHandling = NullValueHandling.Ignore)]
                public bool? DidRename { get; set; }

                /// <summary>
                /// The client has support for sending willRenameFiles requests.
                /// </summary>
                [JsonProperty("willRename", NullValueHandling = NullValueHandling.Ignore)]
                public bool? WillRename { get; set; }

                /// <summary>
                /// The client has support for sending didDeleteFiles notifications.
                /// </summary>
                [JsonProperty("didDelete", NullValueHandling = NullValueHandling.Ignore)]
                public bool? DidDelete { get; set; }

                /// <summary>
                /// The client has support for sending willDeleteFiles requests.
                /// </summary>
                [JsonProperty("willDelete", NullValueHandling = NullValueHandling.Ignore)]
                public bool? WillDelete { get; set; }
            }

            /// <summary>
            /// The client supports applying batch edits
            /// to the workspace by supporting the request
            /// 'workspace/applyEdit'
            /// </summary>
            [JsonProperty("applyEdit", NullValueHandling = NullValueHandling.Ignore)]
            public bool? ApplyEdit { get; set; }

            /// <summary>
            /// Capabilities specific to `WorkspaceEdit`s
            /// </summary>
            [JsonProperty("workspaceEdit", NullValueHandling = NullValueHandling.Ignore)]
            public WorkspaceEditClientCapabilities? WorkspaceEdit { get; set; }

            /// <summary>
            /// Capabilities specific to the `workspace/didChangeConfiguration`
            /// notification.
            /// </summary>
            [JsonProperty("didChangeConfiguration", NullValueHandling = NullValueHandling.Ignore)]
            public DidChangeConfigurationClientCapabilities? DidChangeConfiguration { get; set; }

            /// <summary>
            /// Capabilities specific to the `workspace/didChangeWatchedFiles`
            /// notification.
            /// </summary>
            [JsonProperty("didChangeWatchedFiles", NullValueHandling = NullValueHandling.Ignore)]
            public DidChangeWatchedFilesClientCapabilities? DidChangeWatchedFiles { get; set; }

            /// <summary>
            /// Capabilities specific to the `workspace/symbol` request.
            /// </summary>
            [JsonProperty("symbol", NullValueHandling = NullValueHandling.Ignore)]
            public WorkspaceSymbolClientCapabilities? Symbol { get; set; }

            /// <summary>
            /// Capabilities specific to the `workspace/executeCommand` request.
            /// </summary>
            [JsonProperty("executeCommand", NullValueHandling = NullValueHandling.Ignore)]
            public ExecuteCommandClientCapabilities? ExecuteCommand { get; set; }

            /// <summary>
            /// The client has support for workspace folders.
            /// </summary>
            [Since(3, 6, 0)]
            [JsonProperty("workspaceFolders", NullValueHandling = NullValueHandling.Ignore)]
            public bool? WorkspaceFolders { get; set; }

            /// <summary>
            /// The client supports `workspace/configuration` requests.
            /// </summary>
            [Since(3, 6, 0)]
            [JsonProperty("configuration", NullValueHandling = NullValueHandling.Ignore)]
            public bool? Configuration { get; set; }

            /// <summary>
            /// Capabilities specific to the semantic token requests scoped to the
            /// workspace.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("semanticTokens", NullValueHandling = NullValueHandling.Ignore)]
            public SemanticTokensWorkspaceClientCapabilities? SemanticTokens { get; set; }

            /// <summary>
            /// Capabilities specific to the code lens requests scoped to the
            /// workspace.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("codeLens", NullValueHandling = NullValueHandling.Ignore)]
            public CodeLensWorkspaceClientCapabilities? CodeLens { get; set; }

            /// <summary>
            /// The client has support for file requests/notifications.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("fileOperations", NullValueHandling = NullValueHandling.Ignore)]
            public FileOperationsCapabilities? FileOperations { get; set; }
        }

        public class WindowCapabilities
        {
            /// <summary>
            /// Whether client supports handling progress notifications. If set
            /// servers are allowed to report in `workDoneProgress` property in the
            /// request specific server capabilities.
            /// </summary>
            [Since(3, 15, 0)]
            [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
            public bool? WorkDoneProgress { get; set; }

            /// <summary>
            /// Capabilities specific to the showMessage request
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("showMessage", NullValueHandling = NullValueHandling.Ignore)]
            public ShowMessageRequestClientCapabilities? ShowMessage { get; set; }

            /// <summary>
            /// Client capabilities for the show document request.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("showDocument", NullValueHandling = NullValueHandling.Ignore)]
            public ShowDocumentClientCapabilities? ShowDocument { get; set; }
        }

        public class GeneralCapabilities
        {
            public class StaleRequestSupportCapabilities
            {
                /// <summary>
                /// The client will actively cancel the request.
                /// </summary>
                [JsonProperty("cancel")]
                public bool Cancel { get; set; }

                /// <summary>
                /// The list of requests for which the client
                /// will retry the request if it receives a
                /// response with error code `ContentModified``
                /// </summary>
                [JsonProperty("retryOnContentModified")]
                public IReadOnlyList<string> RetryOnContentModified { get; set; }
            }

            /// <summary>
            /// Client capability that signals how the client
            /// handles stale requests (e.g. a request
            /// for which the client will not process the response
            /// anymore since the information is outdated).
            /// </summary>
            [Since(3, 17, 0)]
            [JsonProperty("staleRequestSupport", NullValueHandling = NullValueHandling.Ignore)]
            public StaleRequestSupportCapabilities? StaleRequestSupport { get; set; }

            /// <summary>
            /// Client capabilities specific to regular expressions.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("regularExpressions", NullValueHandling = NullValueHandling.Ignore)]
            public RegularExpressionsClientCapabilities? RegularExpressions { get; set; }

            /// <summary>
            /// Client capabilities specific to the client's markdown parser.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("markdown", NullValueHandling = NullValueHandling.Ignore)]
            public MarkdownClientCapabilities? Markdown { get; set; }
        }

        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        [JsonProperty("workspace", NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceCapabilities? Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        [JsonProperty("textDocument", NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentClientCapabilities? TextDocument { get; set; }

        /// <summary>
        /// Window specific client capabilities.
        /// </summary>
        [JsonProperty("window", NullValueHandling = NullValueHandling.Ignore)]
        public WindowCapabilities? Window { get; set; }

        /// <summary>
        /// General client capabilities.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("general", NullValueHandling = NullValueHandling.Ignore)]
        public GeneralCapabilities? General { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        [JsonProperty("experimental", NullValueHandling = NullValueHandling.Ignore)]
        public object? Experimental { get; set; }
    }
}
