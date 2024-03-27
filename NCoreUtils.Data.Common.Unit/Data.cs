using System.Text.Json.Serialization;
using NCoreUtils.Data.Json;

namespace NCoreUtils.Data.Common.Unit;

public record ObjectWithDateOnly(
    [property: JsonConverter(typeof(DateOnlyConverter))]
    DateOnly Value
);

public record ObjectWithNullableDateOnly(
    [property: JsonConverter(typeof(NullableDateOnlyConverter))]
    DateOnly? Value
);

public record ObjectWithTimeOnly(
    [property: JsonConverter(typeof(TimeOnlyConverter))]
    TimeOnly Value
);

public record ObjectWithNullableTimeOnly(
    [property: JsonConverter(typeof(NullableTimeOnlyConverter))]
    TimeOnly? Value
);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ObjectWithDateOnly))]
[JsonSerializable(typeof(ObjectWithNullableDateOnly))]
[JsonSerializable(typeof(ObjectWithTimeOnly))]
[JsonSerializable(typeof(ObjectWithNullableTimeOnly))]
public partial class TestDataSerializer : JsonSerializerContext { }