using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Workforce.API;
using Workforce.API.Endpoints;
using Workforce.API.OpenApi;
using Workforce.Infrastructure;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    //options.AddScalarTransformers();
    options.AddSchemaTransformer<ValueObjectTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

});

//builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = "WorkforceApp");

builder.Services.AddValidation();

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader(),
        new MediaTypeApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new UrlSegmentApiVersionReader());
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// .AddOpenApi( options =>
// {
//     options.Document.AddScalarTransformers();
//     options.Document.AddSchemaTransformer<ValueObjectTransformer>();
//     options.Document.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

// });

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", jwtOptions =>
    {
        jwtOptions.Authority = builder.Configuration["JWT:Authority"];
        jwtOptions.Audience = builder.Configuration["JWT:Audience"];
        jwtOptions.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();

builder.AddSqlServerDbContext<WorkforceDbContext>("WorkforceDb",
    configureDbContextOptions: options =>

        options.UseSqlServer(builder.Configuration.GetConnectionString("WorkforceDb"),
            optionsBuilder =>
            {
                optionsBuilder.UseCompatibilityLevel(170);
                optionsBuilder.EnableRetryOnFailure(); // use defaults

            })
            .AddInterceptors(new UniqueConstraintViolationInterceptor())
    );

builder.Services.AddHttpLogging(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.CombineLogs = true;
        o.LoggingFields = HttpLoggingFields.ResponseBody | HttpLoggingFields.ResponseHeaders;
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();//.WithDocumentPerVersion();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
        options.Authentication = new() { PreferredSecuritySchemes = ["Bearer"] };

        var descriptions = app.DescribeApiVersions();

        for ( var i = 0; i < descriptions.Count; i++ )
        {
            var description = descriptions[i];
            var isDefault = i == descriptions.Count - 1;

            options.AddDocument( description.GroupName, description.GroupName, isDefault: isDefault );
        }
    });
}

app.MapEmployeeApi().ToV1();
app.MapSkillApi().ToV1();
app.MapEmployeeSkillApi().ToV1();

await app.RunAsync();



[JsonSerializable(typeof(IEnumerable<Workforce.API.Contracts.Employee>))]
[JsonSerializable(typeof(List<Workforce.API.Contracts.Skill>))]
[JsonSerializable(typeof(Workforce.API.Contracts.EmployeeSkill))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}