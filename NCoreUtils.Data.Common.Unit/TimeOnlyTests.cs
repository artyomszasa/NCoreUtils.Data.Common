using System.Collections;
using System.Text.Json;

namespace NCoreUtils.Data.Common.Unit;

public class TimeOnlyTests
{
    public sealed class Cases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [new TimeOnly(12, 1, 1), "{\"value\":\"12:01:01\"}"];
            yield return [new TimeOnly(12, 12, 12), "{\"value\":\"12:12:12\"}"];
            yield return [new TimeOnly(12, 11, 20), "{\"value\":\"12:11:20\"}"];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Cases))]
    public void Reserialize(TimeOnly value, string expected)
    {
        var raw = JsonSerializer.Serialize(new ObjectWithTimeOnly(value), TestDataSerializer.Default.ObjectWithTimeOnly);
        Assert.Equal(expected, raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithTimeOnly);
        Assert.NotNull(x);
        Assert.Equal(value, x.Value);
    }

    [Theory]
    [ClassData(typeof(Cases))]
    public void ReserializeNullable(TimeOnly value, string expected)
    {
        var raw = JsonSerializer.Serialize(new ObjectWithNullableTimeOnly(value), TestDataSerializer.Default.ObjectWithNullableTimeOnly);
        Assert.Equal(expected, raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithNullableTimeOnly);
        Assert.NotNull(x);
        Assert.Equal(value, x.Value);
    }

    [Fact]
    public void ReserializeNullableNull()
    {
        var raw = JsonSerializer.Serialize(new ObjectWithNullableTimeOnly(default), TestDataSerializer.Default.ObjectWithNullableTimeOnly);
        Assert.Equal("{\"value\":null}", raw);
        var x = JsonSerializer.Deserialize(raw, TestDataSerializer.Default.ObjectWithNullableTimeOnly);
        Assert.NotNull(x);
        Assert.Null(x.Value);
    }

    [Fact]
    public void ErrorHandling()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"cd:ef:gh\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"1d:ef:gh\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12:ef:gh\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12:1f:gh\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12:12:gh\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12:12:1h\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":\"12:12\"}", TestDataSerializer.Default.ObjectWithTimeOnly));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("{\"value\":true}", TestDataSerializer.Default.ObjectWithTimeOnly));
    }
}