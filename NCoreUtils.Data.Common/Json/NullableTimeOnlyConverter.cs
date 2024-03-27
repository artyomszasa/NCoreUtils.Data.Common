namespace NCoreUtils.Data.Json;

public class NullableTimeOnlyConverter : JsonConverter<TimeOnly?>
{
    private static TimeOnlyConverter Converter { get; } = new();

    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default(TimeOnly?),
            _ => Converter.Read(ref reader, typeof(TimeOnly), options)
        };

    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
    {
        if (value is TimeOnly v)
        {
            Converter.Write(writer, v, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}