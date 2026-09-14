using Asp.Versioning.Builder;

namespace Workforce.API.Endpoints;

public abstract class VersionedApiBuilder(IVersionedEndpointRouteBuilder builder)
{
    public IVersionedEndpointRouteBuilder Endpoints => builder;
}

public sealed class VersionedApiBuilder<T>(IVersionedEndpointRouteBuilder builder) : VersionedApiBuilder(builder)
{
    public Type ModelType => typeof(T);

    public Type EnumerableModelType => typeof(IEnumerable<T>);
}
