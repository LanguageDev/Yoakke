using Newtonsoft.Json;
using System;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(ProgressTokenConverter))]
    public readonly struct ProgressToken : IEquatable<ProgressToken>
    {
        public readonly object Value;

        public bool IsString => Value is string;
        public bool IsInt => Value is int;

        public string AsString => (string)Value;
        public int AsInt => (int)Value;

        private ProgressToken(object value)
        {
            Value = value;
        }

        public ProgressToken(string value)
            : this((object)value)
        {
        }

        public ProgressToken(int value)
            : this((object)value)
        {
        }

        public bool Equals(ProgressToken other) => Value.Equals(other.Value);

        public static implicit operator ProgressToken(string value) => new ProgressToken(value);
        public static implicit operator ProgressToken(int value) => new ProgressToken(value);
    }
}
