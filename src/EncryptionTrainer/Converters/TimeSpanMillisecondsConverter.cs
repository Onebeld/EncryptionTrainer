using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EncryptionTrainer.Converters;

public class TimeSpanMillisecondsConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        long milliseconds = reader.GetInt64();
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.TotalMilliseconds);
    }
}