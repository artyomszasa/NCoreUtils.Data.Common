namespace NCoreUtils.Data.Json;

public sealed class DateOnlyConverter : JsonConverter<DateOnly>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseInt4(ReadOnlySpan<byte> input, out int value)
    {
        // NOTE: input mindenképpen 4 karakteres
        if (!(IsAsciiDigit(input[0], out var i0) && IsAsciiDigit(input[1], out var i1) && IsAsciiDigit(input[2], out var i2) && IsAsciiDigit(input[3], out var i3)))
        {
            value = default;
            return false;
        }
        value = unchecked((int)i0 * 1000 + (int)i1 * 100 + (int)i2 * 10 + (int)i3);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    private static bool TryParse(ReadOnlySpan<byte> input, out DateOnly time)
    {
        if (input.Length != 10 || input[4] != (byte)'-' || input[7] != (byte)'-'
            || !TryParseInt4(input[..4], out var year)
            || !TryParseInt2(input.Slice(5, 2), out var month)
            || !TryParseInt2(input.Slice(8, 2), out var day))
        {
            time = default;
            return false;
        }
        time = new DateOnly(year, month, day);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateOnly CopyAndParseOrThrow(in Utf8JsonReader reader)
    {
        if (reader.ValueSequence.Length != 10)
        {
            throw new JsonException($"Unable to read DateOnly value (\"{reader.GetString()}\").");
        }
        Span<byte> buffer = stackalloc byte[10];
        reader.ValueSequence.CopyTo(buffer);
        return ParseOrThrow(in reader, buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateOnly ParseOrThrow(in Utf8JsonReader reader, ReadOnlySpan<byte> span)
        => TryParse(reader.ValueSpan, out var time)
            ? time
            : throw new JsonException($"Unable to read DateOnly value (\"{reader.GetString()}\").");

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // NOTE: both directions are handled here so we use optimized approach
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.HasValueSequence switch
            {
                true => CopyAndParseOrThrow(in reader),
                _ => ParseOrThrow(in reader, reader.ValueSpan)
            },
            var tokenType => throw new JsonException($"Unable to read DateOnly from JSON sequence starting with {tokenType}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        // NOTE: both directions are handled here so we use optimized approach
        var year = value.Year;
        var (yx0, y0) = Math.DivRem(year, 10);
        var (yx1, y1) = Math.DivRem(yx0, 10);
        var (y3, y2) = Math.DivRem(yx1, 10);
        var (m1, m0) = Math.DivRem(value.Month, 10);
        var (d1, d0) = Math.DivRem(value.Day, 10);
        ReadOnlySpan<byte> buffer = [
            ToAsciiDigit(y3),
            ToAsciiDigit(y2),
            ToAsciiDigit(y1),
            ToAsciiDigit(y0),
            (byte)'-',
            ToAsciiDigit(m1),
            ToAsciiDigit(m0),
            (byte)'-',
            ToAsciiDigit(d1),
            ToAsciiDigit(d0)
        ];
        writer.WriteStringValue(buffer);
    }
}