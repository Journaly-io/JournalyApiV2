using Newtonsoft.Json;
using System;

public class TimeOnlyConverter : JsonConverter<TimeOnly?>
{
    public override TimeOnly? ReadJson(JsonReader reader, Type objectType, TimeOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String)
        {
            string timeString = (string)reader.Value;
            return TimeOnly.Parse(timeString);
        }

        throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
    }

    public override void WriteJson(JsonWriter writer, TimeOnly? value, JsonSerializer serializer)
    {
        if (value.HasValue)
        {
            writer.WriteValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNull();
        }
    }
}