// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Workspace
{
    /// <summary>
    /// Matching options for the file operation pattern.
    /// </summary>
    [Since(3, 16, 0)]
    public class FileOperationPatternOptions
    {
        /// <summary>
        /// The pattern should be matched ignoring casing.
        /// </summary>
        [JsonProperty("ignoreCase", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreCase { get; set; }
    }
}
