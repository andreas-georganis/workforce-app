using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

public sealed class ValueObjectJsonConverter<TValueObject, TPrimitive> : JsonConverter<TValueObject>
    where TValueObject : IValueObject<TValueObject, TPrimitive>, IParsable<TValueObject>
{
    public override TValueObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            throw new JsonException($"Cannot convert null to {typeof(TValueObject).Name}.");

        if (reader.TokenType == JsonTokenType.String)
        {
            string? str = reader.GetString();
            if (str is null)
                throw new JsonException("Unexpected null string.");
            return TValueObject.Parse(str, CultureInfo.InvariantCulture);
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            TPrimitive primitive = ReadPrimitive(ref reader);
            return CreateFromPrimitive(primitive);
        }

        throw new JsonException($"Unexpected token {reader.TokenType}");
    }

    private static TPrimitive ReadPrimitive(ref Utf8JsonReader reader)
    {
        // The typeof(TPrimitive) checks are constants – trimming‑safe.
        if (typeof(TPrimitive) == typeof(int))
            return (TPrimitive)(object)reader.GetInt32();
        if (typeof(TPrimitive) == typeof(long))
            return (TPrimitive)(object)reader.GetInt64();
        if (typeof(TPrimitive) == typeof(double))
            return (TPrimitive)(object)reader.GetDouble();
        if (typeof(TPrimitive) == typeof(float))
            return (TPrimitive)(object)reader.GetSingle();
        if (typeof(TPrimitive) == typeof(decimal))
            return (TPrimitive)(object)reader.GetDecimal();

        throw new JsonException($"Unsupported primitive type {typeof(TPrimitive)}");
    }

    private static TValueObject CreateFromPrimitive(TPrimitive value)
    {
        return TValueObject.New(value);
    }

    public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
    {
        var primitive = value.Value;
        switch (primitive)
        {
            case int i: writer.WriteNumberValue(i); break;
            case long l: writer.WriteNumberValue(l); break;
            case double d: writer.WriteNumberValue(d); break;
            case float f: writer.WriteNumberValue(f); break;
            case decimal m: writer.WriteNumberValue(m); break;
            case string s: writer.WriteStringValue(s); break;
            default:
                writer.WriteStringValue(value.ToString()); // fallback
                break;
        }
    }
}