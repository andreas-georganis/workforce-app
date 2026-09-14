using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Workforce.Web.Client.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddScoped(_ => new HttpClient {
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    DefaultRequestHeaders = { { "X-Api-Version", "1.0" } }
});
builder.Services.AddScoped<IEmployeeClient, EmployeeClient>();
builder.Services.AddScoped<ISkillClient, SkillClient>();
builder.Services.AddScoped<IEmployeeSkillClient, EmployeeSkillClient>();

await builder.Build().RunAsync();
