// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Diagnostics
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#publishDiagnosticsParams.
    /// </summary>
    public class PublishDiagnosticsParams
    {
        /// <summary>
        /// The URI for which diagnostic information is reported.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// Optional the version number of the document the diagnostics are published
        /// for.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Version { get; set; }

        /// <summary>
        /// An array of diagnostic information items.
        /// </summary>
        [JsonProperty("diagnostics")]
        public IReadOnlyList<Diagnostic> Diagnostics { get; set; } = Array.Empty<Diagnostic>();
    }
}
