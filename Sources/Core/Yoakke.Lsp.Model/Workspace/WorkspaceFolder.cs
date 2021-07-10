// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Workspace
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workspaceFolder.
    /// </summary>
    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// The name of the workspace folder. Used to refer to this
        /// workspace folder in the user interface.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
