using System.Text.Json;
using System.Text.Json.Serialization;

namespace TenBis.Enums
{
    internal class NotifyTypeConverter : JsonConverter<NotifyType>
    {
        public override NotifyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string enumValue = reader.GetString();
            return Enum.Parse<NotifyType>(enumValue, ignoreCase: true);
        }

        public override void Write(Utf8JsonWriter writer, NotifyType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
