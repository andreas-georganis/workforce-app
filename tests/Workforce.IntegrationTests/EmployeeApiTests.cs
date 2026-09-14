using Microsoft.AspNetCore.Mvc.Testing;

namespace Workforce.IntegrationTests;

public sealed class EmployeeApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact]
    public async Task ProtectedEmployeesEndpointRequiresBearerToken()
    {
        using var response = await client.GetAsync("/api/employees/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
