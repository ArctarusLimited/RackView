using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Models.Config.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coordinator.Helpers
{
    public class DynamicDictConverter<T> : CustomCreationConverter<T> where T : new()
    {
        public override T Create(Type objectType)
        {
            return new T();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject ||
                reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);
            return serializer.Deserialize(reader);
        }
    }
}
