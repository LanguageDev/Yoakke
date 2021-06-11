using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Workspace;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    public class WorkspaceEditClientCapabilities
    {
        public class ChangeAnnotationSupportCapabilities
        {
            /// <summary>
            /// Whether the client groups edits with equal labels into tree nodes,
            /// for instance all edits labelled with "Changes in Strings" would
            /// be a tree node.
            /// </summary>
            [JsonProperty("groupsOnLabel", NullValueHandling = NullValueHandling.Ignore)]
            public bool? GroupsOnLabel { get; set; }
        }

        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        [JsonProperty("documentChanges", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DocumentChanges { get; set; }

        /// <summary>
        /// The resource operations the client supports. Clients should at least
        /// support 'create', 'rename' and 'delete' files and folders.
        /// </summary>
        [Since(3, 13, 0)]
        [JsonProperty("resourceOperations", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<ResourceOperationKind>? ResourceOperations { get; set; }

        /// <summary>
        /// The failure handling strategy of a client if applying the workspace edit
        /// fails.
        /// </summary>
        [Since(3, 13, 0)]
        [JsonProperty("failureHandling", NullValueHandling = NullValueHandling.Ignore)]
        public FailureHandlingKind? FailureHandling { get; set; }

        /// <summary>
        /// Whether the client normalizes line endings to the client specific
        /// setting.
        /// If set to `true` the client will normalize line ending characters
        /// in a workspace edit to the client specific new line character(s).
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("normalizesLineEndings", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NormalizesLineEndings { get; set; }

        /// <summary>
        /// Whether the client in general supports change annotations on text edits,
        /// create file, rename file and delete file changes.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("changeAnnotationSupport", NullValueHandling = NullValueHandling.Ignore)]
        public ChangeAnnotationSupportCapabilities? ChangeAnnotationSupport { get; set; }
    }
}
