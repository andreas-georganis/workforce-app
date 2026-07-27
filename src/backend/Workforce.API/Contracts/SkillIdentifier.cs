using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Workforce.Domain.Model;

namespace Workforce.API.Contracts;

[JsonConverter(typeof(ParsableJsonConverter<SkillIdentifier>))]
public readonly record struct SkillIdentifier(SkillId? Id, SkillName? Name) : IParsable<SkillIdentifier>
{
    public static SkillIdentifier Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SkillIdentifier result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        if (SkillId.TryParse(s, provider, out var skillId))
        {
            result = new SkillIdentifier(skillId, null);
            return true;
        }

        if (SkillName.TryParse(s, provider, out var skillName))
        {
            result = new SkillIdentifier(null, skillName);
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return Id?.ToString() ?? Name?.ToString() ?? string.Empty;
    }
}