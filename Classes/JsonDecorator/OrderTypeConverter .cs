using Newtonsoft.Json;
using System;
using CriptoExchengLib.Interfaces;
using System.Linq;

namespace CriptoExchengLib.Classes.JsonDecorator
{
    public class OrderTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetInterfaces().Contains(typeof(IOrderType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((IOrderType)value).Value);
        }
    }
}
