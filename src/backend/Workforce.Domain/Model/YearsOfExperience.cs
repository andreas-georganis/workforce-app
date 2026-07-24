using System.Diagnostics.CodeAnalysis;

namespace Workforce.Domain.Model;


public sealed class YearsOfExperience : IParsable<YearsOfExperience>
{
    public YearsOfExperience(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public int Value { get; }

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