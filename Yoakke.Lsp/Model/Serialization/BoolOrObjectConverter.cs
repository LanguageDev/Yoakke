using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Serialization
{
    public class BoolOrObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
               objectType.IsGenericType
            && objectType.GetGenericTypeDefinition() == typeof(BoolOrObject<>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Boolean)
            { 
                return Activator.CreateInstance(objectType, (bool)reader.Value)!;
            }
            else
            {
                var tType = objectType.GenericTypeArguments[0];
                return Activator.CreateInstance(objectType, serializer.Deserialize(reader, tType))!;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();
            var subValue = valueType.GetField("Value")!.GetValue(value);
            if (subValue is bool boolValue) writer.WriteValue(boolValue);
            else serializer.Serialize(writer, subValue);
        }
    }
}
