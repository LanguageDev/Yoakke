// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// Represents a related message and source code location for a diagnostic.
    /// This should be used to point to code locations that cause or are related to
    /// a diagnostics, e.g when duplicating a symbol in a scope.
    /// </summary>
    public class DiagnosticRelatedInformation
    {
        /// <summary>
        /// The location of this related diagnostic information.
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }
        /// <summary>
        /// The message of this related diagnostic information.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
