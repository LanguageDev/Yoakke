using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.LanguageFeatures;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class CodeActionOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// CodeActionKinds that this server may return.
        ///
        /// The list of kinds may be generic, such as `CodeActionKind.Refactor`,
        /// or the server may list out every specific kind they provide.
        /// </summary>
        [JsonProperty("codeActionKinds", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<CodeActionKind>? CodeActionKinds { get; set; }

        /// <summary>
        /// The server provides support to resolve additional
        /// information for a code action.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("resolveProvider", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ResolveProvider { get; set; }
    }
}
