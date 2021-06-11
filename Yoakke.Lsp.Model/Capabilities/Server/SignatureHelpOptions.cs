using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class SignatureHelpOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// The characters that trigger signature help
        /// automatically.
        /// </summary>
        [JsonProperty("triggerCharacters", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string>? TriggerCharacters { get; set; }

        /// <summary>
        /// List of characters that re-trigger signature help.
        ///
        /// These trigger characters are only active when signature help is already
        /// showing. All trigger characters are also counted as re-trigger
        /// characters.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("retriggerCharacters", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string>? RetriggerCharacters { get; set; }
    }
}
