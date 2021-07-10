// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#workDoneProgressOptions.
    /// </summary>
    public interface IWorkDoneProgressOptions
    {
        /// <summary>
        /// True, if work done progress reporting is supported.
        /// </summary>
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }
    }
}
