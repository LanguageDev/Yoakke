// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#documentUri.
    /// </summary>
    [JsonConverter(typeof(DocumentUriConverter))]
    public readonly struct DocumentUri : IEquatable<DocumentUri>
    {
        /// <summary>
        /// The raw URI string.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentUri"/> struct.
        /// </summary>
        /// <param name="value">The raw URI string.</param>
        public DocumentUri(string value)
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        public bool Equals(DocumentUri other) => this.Value.Equals(other.Value);

        /// <summary>
        /// Implicitly converts a <see cref="string"/> to a <see cref="DocumentUri"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to convert.</param>
        public static implicit operator DocumentUri(string value) => new(value);
    }
}
