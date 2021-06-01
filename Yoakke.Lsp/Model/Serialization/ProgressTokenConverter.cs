using Newtonsoft.Json;
using System;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization
{
    public class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        public override ProgressToken ReadJson(JsonReader reader, Type objectType, ProgressToken existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            reader.TokenType == JsonToken.String
                ? new ProgressToken(reader.ReadAsString())
                : new ProgressToken(reader.ReadAsInt32()!.Value);

        public override void WriteJson(JsonWriter writer, ProgressToken value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
