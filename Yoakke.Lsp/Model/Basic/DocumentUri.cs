using Newtonsoft.Json;
using System;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(DocumentUriConverter))]
    public readonly struct DocumentUri : IEquatable<DocumentUri>
    {
        public readonly string Value;

        public DocumentUri(string value)
        {
            Value = value;
        }

        public bool Equals(DocumentUri other) => Value.Equals(other.Value);
    }
}
