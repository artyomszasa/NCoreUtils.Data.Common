namespace NCoreUtils.Data.Json;

public sealed class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ToAsciiDigit(int b)
        => unchecked((byte)(b + '0'));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAsciiDigit(byte b, out uint value)
    {
        value = unchecked((uint)b - (byte)'0');
        return value <= 9u;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseInt2(ReadOnlySpan<byte> input, out int value)
    {
        // NOTE: input mindenképpen 2 karakteres
        if (!(IsAsciiDigit(input[0], out var i0) && IsAsciiDigit(input[1], out var i1)))
        {
            value = default;
            return false;
        }
        value = unchecked((int)i0 * 10 + (int)i1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    private static bool TryParse(ReadOnlySpan<byte> input, out TimeOnly time)
    {
        if (input.Length != 8 || input[2] != (byte)':' || input[5] != (byte)':'
            || !TryParseInt2(input[..2], out var hour)
            || !TryParseInt2(input.Slice(3, 2), out var minute)
            || !TryParseInt2(input.Slice(6, 2), out var second))
        {
            time = default;
            return false;
        }
        time = new TimeOnly(hour, minute, second);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // NOTE: mivel mindkét irány itt van meghatározva, specializált parser/stringifier-t használjuk.
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.HasValueSequence switch
            {
                true => CopyAndParseOrThrow(in reader),
                _ => ParseOrThrow(in reader, reader.ValueSpan)
            },
            var tokenType => throw new JsonException($"Unable to read TimeOnly from JSON sequence starting with {tokenType}")
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TimeOnly CopyAndParseOrThrow(in Utf8JsonReader reader)
        {
            if (reader.ValueSequence.Length != 8)
            {
                throw new JsonException($"Unable to read TimeOnly value (\"{reader.GetString()}\").");
            }
            Span<byte> buffer = stackalloc byte[8];
            reader.ValueSequence.CopyTo(buffer);
            return ParseOrThrow(in reader, buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TimeOnly ParseOrThrow(in Utf8JsonReader reader, ReadOnlySpan<byte> span)
            => TryParse(reader.ValueSpan, out var time)
                ? time
                : throw new JsonException($"Unable to read TimeOnly value (\"{reader.GetString()}\").");
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        // NOTE: mivel mindkét irány itt van meghatározva, specializált parser/stringifier-t használjuk.
        var (h1, h0) = Math.DivRem(value.Hour, 10);
        var (m1, m0) = Math.DivRem(value.Minute, 10);
        var (s1, s0) = Math.DivRem(value.Second, 10);
        ReadOnlySpan<byte> buffer =
        [
            ToAsciiDigit(h1),
            ToAsciiDigit(h0),
            (byte)':',
            ToAsciiDigit(m1),
            ToAsciiDigit(m0),
            (byte)':',
            ToAsciiDigit(s1),
            ToAsciiDigit(s0),
        ];
        writer.WriteStringValue(buffer);
    }
}