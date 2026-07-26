using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

[JsonConverter(typeof(ValueObjectJsonConverter<Email, string>))]
public sealed partial record Email : IValueObject<Email, string>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        Value = value.Trim().ToLowerInvariant();

        if (!MyRegex.IsMatch(Value))
            throw new ArgumentException($"Invalid email format: {Value}");
    }

    public override string ToString() => Value;

    public static Email Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Email result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null;
            return false;
        }

        try
        {
            result = new Email(s);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public static Email New(string value)
        => new(value);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex MyRegex { get; }
}