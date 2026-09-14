using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Workforce.Web.Client.Clients;
using Workforce.Web.Clients;
using Workforce.Web;
using Workforce.Web.Components;
using Workforce.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication("oidc")
.AddOpenIdConnect("oidc", oidcOptions =>
{
    // For the following OIDC settings, any line that's commented out
    // represents a DEFAULT setting. If you adopt the default, you can
    // remove the line if you wish.

    //oidcOptions.PushedAuthorizationBehavior = PushedAuthorizationBehavior.UseIfAvailable;

    oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    //oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
    //oidcOptions.CallbackPath = new PathString("/signin-oidc");
    //oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
    //oidcOptions.RemoteSignOutPath = new PathString("/signout-oidc");


    oidcOptions.Authority = builder.Configuration["OIDC:Authority"];
    oidcOptions.ClientId = builder.Configuration["OIDC:ClientId"];
    oidcOptions.ClientSecret = builder.Configuration["OIDC:ClientSecret"];
    oidcOptions.ResponseType = OpenIdConnectResponseType.Code;


    oidcOptions.MapInboundClaims = false;
    oidcOptions.TokenValidationParameters.NameClaimType = "name";
    oidcOptions.TokenValidationParameters.RoleClaimType = "roles";

    // oidcOptions.Events.OnRedirectToIdentityProviderForSignOut = context =>
    // {
    //     var idTokenHint = context.ProtocolMessage.IdTokenHint;
    //     if (string.IsNullOrWhiteSpace(idTokenHint))
    //     {
    //         return Task.CompletedTask;
    //     }

    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     if (!tokenHandler.CanReadToken(idTokenHint))
    //     {
    //         context.ProtocolMessage.IdTokenHint = null;
    //         return Task.CompletedTask;
    //     }

    //     var token = tokenHandler.ReadJwtToken(idTokenHint);

    //     if (!string.Equals(token.Issuer?.TrimEnd('/'), oidcOptions.Authority?.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
    //     {
    //         context.ProtocolMessage.IdTokenHint = null;
    //         return Task.CompletedTask;
    //     }

    //     if (!token.Audiences.Contains(oidcOptions.ClientId, StringComparer.Ordinal))
    //     {
    //         context.ProtocolMessage.IdTokenHint = null;
    //         return Task.CompletedTask;
    //     }

    //     return Task.CompletedTask;
    // };

    // ........................................................................
    // OIDC connect options set later via ConfigureCookieOidc
    //
    // (1) The "offline_access" scope is required for the refresh token.
    //
    // (2) SaveTokens is set to true, which saves the access and refresh tokens
    // in the cookie, so the app can authenticate requests for weather data and
    // cookie, so the app can authenticate requests for weather data and
    // use the refresh token to obtain a new access token on access token
    // expiration.
    // ........................................................................

    oidcOptions.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

// ConfigureCookieOidc attaches a cookie OnValidatePrincipal callback to get
// a new access token when the current one expires, and reissue a cookie with the
// new access token saved inside. If the refresh fails, the user will be signed
// out. OIDC connect options are set for saving tokens and the offline access
// scope.
builder.Services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, "oidc");

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TokenHandler>();

builder.Services.AddHttpClient<IEmployeeClient, Workforce.Web.Clients.EmployeeClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri("http://workforce-api");
    httpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<ISkillClient, Workforce.Web.Clients.SkillClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri("http://workforce-api");
    httpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<IEmployeeSkillClient, Workforce.Web.Clients.EmployeeSkillClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri("http://workforce-api");
    httpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
})
.AddHttpMessageHandler<TokenHandler>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapDefaultEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Workforce.Web.Client._Imports).Assembly);

app.MapGroup("/authentication").MapAuthApi();

app.MapEmployeeApi();
app.MapSkillApi();
app.MapEmployeeSkillApi();

app.Run();
