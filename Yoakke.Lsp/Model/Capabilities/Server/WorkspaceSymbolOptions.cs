﻿using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class WorkspaceSymbolOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }
    }
}
