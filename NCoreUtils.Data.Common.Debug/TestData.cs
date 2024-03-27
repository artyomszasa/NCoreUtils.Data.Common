using System.Text.Json.Serialization;
using NCoreUtils.Data.Json;

namespace NCoreUtils;

public record TestData(
    [property: JsonConverter(typeof(DateOnlyConverter))]
    DateOnly Date
);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(TestData))]
public partial class TestDataSerializer : JsonSerializerContext { }