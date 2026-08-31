using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<SkillId>))]
public readonly record struct SkillId(Guid Value) : IParsable<SkillId>
{
    public static SkillId New()
        => new(UUIDNext.Uuid.NewDatabaseFriendly(UUIDNext.Database.SqlServer));

    public static SkillId Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SkillId result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return true;
        }

        if (!Guid.TryParse(s, out var id))
        {
            result = default;
            return false;
        }

        result = new(id);
        return true;
    }

    override public string ToString() => Value.ToString();
}