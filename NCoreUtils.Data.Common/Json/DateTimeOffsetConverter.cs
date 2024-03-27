using System.Globalization;

namespace NCoreUtils.Data.Json;

public sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Number => new DateTimeOffset(reader.GetInt64(), TimeSpan.Zero),
            JsonTokenType.String => DateTimeOffset.TryParseExact(reader.GetString(), "o", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var value)
                ? value
                : DateTimeOffset.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out value)
                    ? value
                    : throw new FormatException($"Unable to convert \"{reader.GetString()}\" to DateTimeOffset"),
            var token => throw new JsonException($"Unable to convert sequence starting with {token} to DateTimeOffset")
        };

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("o", CultureInfo.InvariantCulture));
}