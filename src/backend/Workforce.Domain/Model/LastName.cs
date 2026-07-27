using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Workforce.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<LastName>))]
public sealed class LastName : IParsable<LastName>
{
    public LastName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static LastName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LastName result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        result = new LastName(s);
        return true;
    }

    public override string ToString() => Value;
}