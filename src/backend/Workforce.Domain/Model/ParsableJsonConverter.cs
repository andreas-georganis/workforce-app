using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

public sealed class ParsableJsonConverter<T> : JsonConverter<T?> where T : IParsable<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringValue = reader.GetString();
        if (stringValue is null)
            return default;

        if (T.TryParse(stringValue, CultureInfo.InvariantCulture, out T? result))
            return result;

        throw new JsonException($"Cannot parse '{stringValue}' to {typeof(T).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}