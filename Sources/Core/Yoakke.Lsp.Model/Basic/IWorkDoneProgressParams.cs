// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-3-17/#workDoneProgressParams.
    /// </summary>
    public interface IWorkDoneProgressParams
    {
        /// <summary>
        /// An optional token that a server can use to report work done progress.
        /// </summary>
        [JsonProperty("workDoneToken", NullValueHandling = NullValueHandling.Ignore)]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
