// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#executeCommandOptions.
    /// </summary>
    public class ExecuteCommandOptions : IWorkDoneProgressOptions
    {
        /// <inheritdoc/>
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// The commands to be executed on the server.
        /// </summary>
        [JsonProperty("commands")]
        public IReadOnlyList<string> Commands { get; set; }
    }
}
