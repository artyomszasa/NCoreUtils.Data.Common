namespace NCoreUtils.Data.Json;

public class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    private static DateTimeOffsetConverter Converter { get; } = new();

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default(DateTimeOffset?),
            _ => Converter.Read(ref reader, typeof(DateTimeOffset), options)
        };

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is DateTimeOffset v)
        {
            Converter.Write(writer, v, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}