using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(DocumentUriConverter))]
    public readonly struct DocumentUri : IEquatable<DocumentUri>
    {
        public readonly string Value;

        public DocumentUri(string value)
        {
            this.Value = value;
        }

        public bool Equals(DocumentUri other) => this.Value.Equals(other.Value);

        public static implicit operator DocumentUri(string value) => new DocumentUri(value);
    }
}
