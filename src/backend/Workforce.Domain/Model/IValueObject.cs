using System.Text.Json.Serialization;

namespace Workforce.Domain.Model;

public interface IValueObject<TSelf, TPrimitive> : IEquatable<TSelf>, IParsable<TSelf>
    where TSelf : IValueObject<TSelf, TPrimitive>, IParsable<TSelf>
{
    static abstract TSelf New(TPrimitive value);

    TPrimitive Value { get; }

    bool IEquatable<TSelf>.Equals(TSelf? other) =>
        other is not null && EqualityComparer<TPrimitive>.Default.Equals(Value, other.Value);
}