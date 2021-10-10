using Newtonsoft.Json;
using System;
using System.Globalization;

namespace UnityBotService.Serialization
{
    public class JsonDateConverter : JsonConverter<DateTime>
    {
        private const string Format = "dd/MM/yyyy";

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Format));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return default;
            }

            var s = reader.Value.ToString();
            if (DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return default;
        }
    }
}