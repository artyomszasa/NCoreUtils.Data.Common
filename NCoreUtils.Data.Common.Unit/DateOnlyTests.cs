using System.Collections;
using System.Text.Json;

namespace NCoreUtils.Data.Common.Unit;

public class DateOnlyTests
{
    public sealed class Cases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [new DateOnly(2000, 1, 1), "{\"value\":\"2000-01-01\"}"];
            yield return [new DateOnly(2000, 12, 12), "{\"value\":\"2000-12-12\"}"];
            yield return [new DateOnly(2000, 11, 20), "{\"value\":\"2000-11-20\"}"];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Cases))]
    public void Reserialize(DateOnly value, string expected)
    {
        var raw = JsonSerializer.Serialize(new ObjectWithDateOnly(value), TestDataSerializer.Default.ObjectWithDateOnly);
        Assert.Equal(expected, raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithDateOnly);
        Assert.NotNull(x);
        Assert.Equal(value, x.Value);
    }

    [Theory]
    [ClassData(typeof(Cases))]
    public void ReserializeNullable(DateOnly value, string expected)
    {
        var raw = JsonSerializer.Serialize(new ObjectWithNullableDateOnly(value), TestDataSerializer.Default.ObjectWithNullableDateOnly);
        Assert.Equal(expected, raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithNullableDateOnly);
        Assert.NotNull(x);
        Assert.Equal(value, x.Value);
    }

    [Fact]
    public void ReserializeNullableNull()
    {
        var raw = JsonSerializer.Serialize(new ObjectWithNullableDateOnly(default), TestDataSerializer.Default.ObjectWithNullableDateOnly);
        Assert.Equal("{\"value\":null}", raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithNullableDateOnly);
        Assert.NotNull(x);
        Assert.Null(x.Value);
    }

    [Fact]
    public void ErrorHandling()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"abcd-ef-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1bcd-ef-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12cd-ef-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"123d-ef-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1234-ef-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1234-1f-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1234-12-gh\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1234-12-1h\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1234-12\"}", TestDataSerializer.Default.ObjectWithDateOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":true}", TestDataSerializer.Default.ObjectWithDateOnly));
    }
}