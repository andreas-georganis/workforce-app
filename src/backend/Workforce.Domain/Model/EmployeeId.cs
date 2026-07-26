using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Workforce.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<EmployeeId>))]
public readonly record struct EmployeeId : IParsable<EmployeeId>
{
    public static EmployeeId New() => new(Guid.CreateVersion7());

    public EmployeeId(Guid value)
    {
       Value = value;
    }

    public Guid Value {get;}

    public static EmployeeId Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out EmployeeId result)
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