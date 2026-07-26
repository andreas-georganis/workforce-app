using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Workforce.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<FirstName>))]
public sealed class FirstName : IParsable<FirstName>
{
    public FirstName(string value)
    {
        Value = value;
    }

    public string Value { get; }


    public static FirstName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FirstName result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        result = new FirstName(s);
        return true;
    }

    public override string ToString() => Value;
}