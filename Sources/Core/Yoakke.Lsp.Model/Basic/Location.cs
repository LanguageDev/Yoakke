// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// The https://microsoft.github.io/language-server-protocol/specifications/specification-3-17/#location.
    /// </summary>
    public readonly struct Location
    {
        /// <summary>
        /// The <see cref="DocumentUri"/> for the file.
        /// </summary>
        [JsonProperty("uri")]
        public readonly DocumentUri Uri;

        /// <summary>
        /// The <see cref="Basic.Range"/> in the file.
        /// </summary>
        [JsonProperty("range")]
        public readonly Range Range;

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        /// <param name="uri">The <see cref="DocumentUri"/> of the file.</param>
        /// <param name="range">The <see cref="Range"/> in the file.</param>
        public Location(DocumentUri uri, Range range)
        {
            this.Uri = uri;
            this.Range = range;
        }
    }
}
