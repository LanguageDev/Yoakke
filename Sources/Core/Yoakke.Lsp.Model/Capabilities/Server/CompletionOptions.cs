// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    /// <summary>
    /// Completion options.
    /// </summary>
    public class CompletionOptions : IWorkDoneProgressOptions
    {
        public class CompletionItemOptions
        {
            /// <summary>
            /// The server has support for completion item label
            /// details (see also `CompletionItemLabelDetails`) when receiving
            /// a completion item in a resolve call.
            /// </summary>
            [Since(3, 17, 0)]
            [JsonProperty("labelDetailsSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? LabelDetailsSupport { get; set; }
        }

        /// <inheritdoc/>
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// Most tools trigger completion request automatically without explicitly
        /// requesting it using a keyboard shortcut (e.g. Ctrl+Space). Typically they
        /// do so when the user starts to type an identifier. For example if the user
        /// types `c` in a JavaScript file code complete will automatically pop up
        /// present `console` besides others as a completion item. Characters that
        /// make up identifiers don't need to be listed here.
        ///
        /// If code complete should automatically be trigger on characters not being
        /// valid inside an identifier (for example `.` in JavaScript) list them in
        /// `triggerCharacters`.
        /// </summary>
        [JsonProperty("triggerCharacters", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string>? TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. This field
        /// can be used if clients don't support individual commit characters per
        /// completion item. See client capability
        /// `completion.completionItem.commitCharactersSupport`.
        ///
        /// If a server provides both `allCommitCharacters` and commit characters on
        /// an individual completion item the ones on the completion item win.
        /// </summary>
        [Since(3, 2, 0)]
        [JsonProperty("allCommitCharacters", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string>? AllCommitCharacters { get; set; }

        /// <summary>
        /// The server provides support to resolve additional
        /// information for a completion item.
        /// </summary>
        [JsonProperty("resolveProvider", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ResolveProvider { get; set; }

        /// <summary>
        /// The server supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [Since(3, 17, 0)]
        [JsonProperty("completionItem", NullValueHandling = NullValueHandling.Ignore)]
        public CompletionItemOptions? CompletionItem { get; set; }
    }
}
