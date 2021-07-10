// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#documentFilter.
    /// </summary>
    public class DocumentFilter
    {
        /// <summary>
        /// A language id, like `typescript`.
        /// </summary>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string? Language { get; set; }

        /// <summary>
        /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
        /// </summary>
        [JsonProperty("scheme", NullValueHandling = NullValueHandling.Ignore)]
        public string? Scheme { get; set; }

        /// <summary>
        /// A glob pattern, like `*.{ts,js}`.
        ///
        /// Glob patterns can have the following syntax:
        /// - `*` to match one or more characters in a path segment
        /// - `?` to match on one character in a path segment
        /// - `**` to match any number of path segments, including none
        /// - `{}` to group sub patterns into an OR expression. (e.g. `**?/*.{ts,js}`
        /// matches all TypeScript and JavaScript files)
        /// - `[]` to declare a range of characters to match in a path segment
        /// (e.g., `example.[0-9]` to match on `example.0`, `example.1`, .)
        /// - `[!...]` to negate a range of characters to match in a path segment
        /// (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
        /// not `example.0`).
        /// </summary>
        [JsonProperty("pattern", NullValueHandling = NullValueHandling.Ignore)]
        public string? Pattern { get; set; }
    }
}
