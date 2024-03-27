namespace NCoreUtils.Data.Json;

public class NullableDateOnlyConverter : JsonConverter<DateOnly?>
{
    private static DateOnlyConverter Converter { get; } = new();

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => default(DateOnly?),
            _ => Converter.Read(ref reader, typeof(DateOnly), options)
        };

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is DateOnly v)
        {
            Converter.Write(writer, v, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}