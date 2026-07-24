using System.Diagnostics.CodeAnalysis;

namespace Workforce.Domain.Model;


public readonly record struct SkillId(Guid Value) : IParsable<SkillId>
{
    public static SkillId New() => new(Guid.CreateVersion7());

    public static SkillId Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SkillId result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (!Guid.TryParse(s, out var id))
        {
            result = default;
            return false;
        }

        result = new(id);
        return true;
    }
}