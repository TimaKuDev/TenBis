using System.Text.Json;
using System.Text.Json.Serialization;

namespace TenBis.Enums
{
    internal class BrowserTypeConverter : JsonConverter<BrowserType>
    {
        public override BrowserType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string enumValue = reader.GetString();
            return Enum.Parse<BrowserType>(enumValue, ignoreCase: true);
        }

        public override void Write(Utf8JsonWriter writer, BrowserType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
