using System.Text.RegularExpressions;

namespace Workforce.Domain.Model;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        Value = value.Trim().ToLowerInvariant();

        if (!Regex.IsMatch(Value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException($"Invalid email format: {Value}");
    }

    public override string ToString() => Value;
}