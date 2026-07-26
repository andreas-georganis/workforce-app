using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<SkillName>))]
public sealed class SkillName : IParsable<SkillName>
{
    public SkillName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static SkillName New(string value)
        => new(value);

    public static SkillName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SkillName result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        result = new SkillName(s);
        return true;
    }

    public override string ToString()
    {
        return Value;
    }
}