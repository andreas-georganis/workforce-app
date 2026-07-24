using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Workforce.API.Contracts;
using Workforce.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = "WorkforceApp");

builder.Services.AddValidation();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", jwtOptions =>
    {
        jwtOptions.Authority = "{AUTHORITY}";
        jwtOptions.Audience = "{AUDIENCE}";
    });

builder.Services.AddAuthorization();

builder.AddSqlServerDbContext<WorkforceDbContext>("WorkforceDb",
    configureDbContextOptions: options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("WorkforceDb"),
            optionsBuilder =>
            {
                optionsBuilder.UseCompatibilityLevel(170);
                optionsBuilder.EnableRetryOnFailure(); // use defaults
            }));

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
        options.Authentication = new() { PreferredSecuritySchemes = [IdentityConstants.BearerScheme] };
    });
}

app.Run();



[JsonSerializable(typeof(Workforce.API.Contracts.Employee))]
[JsonSerializable(typeof(Workforce.API.Contracts.Skill))]
[JsonSerializable(typeof(Workforce.API.Contracts.EmployeeSkill))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
