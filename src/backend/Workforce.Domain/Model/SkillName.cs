using System.Diagnostics.CodeAnalysis;

namespace Workforce.Domain.Model;

public sealed class SkillName : IParsable<SkillName>
{
    public SkillName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static SkillName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SkillName result)
    {
        throw new NotImplementedException();
    }
}