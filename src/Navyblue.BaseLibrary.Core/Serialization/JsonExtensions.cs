using System.Text.Json;

namespace Navyblue.BaseLibrary.Serialization;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new(JsonSerializerDefaults.Web);
    public static string ToJson<T>(this T value, JsonSerializerOptions? options = null) => JsonSerializer.Serialize(value, options ?? DefaultOptions);
    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null) => JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
}
