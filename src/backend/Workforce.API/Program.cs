using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Workforce.API;
using Workforce.API.Contracts;
using Workforce.API.Endpoints;
using Workforce.API.OpenApi;
using Workforce.Infrastructure;
using WorkForce.API.Endpoints;

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
    options.AddSchemaTransformer<ValueObjectTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

//builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = "WorkforceApp");

builder.Services.AddValidation();

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddProblemDetails();

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
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
        options.Authentication = new() { PreferredSecuritySchemes = ["Bearer"] };
    });
}

var apis = app.MapGroup("api/");

apis.MapEmployeeApi();
apis.MapSkillApi();

app.Run();



[JsonSerializable(typeof(IEnumerable<Workforce.API.Contracts.Employee>))]
[JsonSerializable(typeof(List<Workforce.API.Contracts.Skill>))]
[JsonSerializable(typeof(Workforce.API.Contracts.EmployeeSkill))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}