using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization
{
    public class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        public override ProgressToken ReadJson(JsonReader reader, Type objectType, ProgressToken existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            reader.TokenType == JsonToken.String
                ? new ProgressToken((string)reader.Value)
                : new ProgressToken((int)reader.Value);

        public override void WriteJson(JsonWriter writer, ProgressToken value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
