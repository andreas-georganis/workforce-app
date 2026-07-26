using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Workforce.Domain.Model;

[JsonConverter(typeof(ValueObjectJsonConverter<YearsOfExperience, int>))]
public sealed class YearsOfExperience : IValueObject<YearsOfExperience, int>
{
    public YearsOfExperience(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public int Value { get; }

    public static YearsOfExperience New(int value)
        => new(value);

    public static YearsOfExperience Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out YearsOfExperience result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (!int.TryParse(s, out var value))
        {
            result = default;
            return false;
        }

        result = new(value);
        return true;
    }
}