using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Workforce.Domain.Model;

namespace Workforce.API.OpenApi;

public class ValueObjectTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(YearsOfExperience))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
        }

        if (context.JsonTypeInfo.Type == typeof(EmployeeId) || context.JsonTypeInfo.Type == typeof(SkillId))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "uuid";
        }

        return Task.CompletedTask;
    }
}
