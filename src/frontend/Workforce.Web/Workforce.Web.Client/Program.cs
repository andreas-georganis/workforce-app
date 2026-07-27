using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Workforce.Web.Client.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IEmployeeClient, EmployeeClient>();
builder.Services.AddScoped<ISkillClient, SkillClient>();

await builder.Build().RunAsync();
