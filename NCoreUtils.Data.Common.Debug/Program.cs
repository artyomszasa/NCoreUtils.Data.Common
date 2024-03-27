// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using NCoreUtils;

var now = DateTimeOffset.Now;
var today = new DateOnly(now.Year, now.Month, now.Day);

string json = string.Empty;
TestData data = new(today);
for (var i = 0; i < 100; ++i)
{
    json = JsonSerializer.Serialize(new TestData(today), TestDataSerializer.Default.TestData);
    data = JsonSerializer.Deserialize(json, TestDataSerializer.Default.TestData)!;
}
Console.WriteLine(json);
Console.WriteLine(data);