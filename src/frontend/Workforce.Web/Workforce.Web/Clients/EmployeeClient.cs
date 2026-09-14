using System.Net.Http.Json;
using Workforce.Web.Client.Clients;

namespace Workforce.Web.Clients;

public sealed class EmployeeClient(HttpClient httpClient) : IEmployeeClient
{
    public async Task<IReadOnlyList<Employee>> GetEmployees(string? skillIdentifier = null, bool includeMatchingSkill = true, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(skillIdentifier))
        {
            query.Add($"skill={Uri.EscapeDataString(skillIdentifier)}");
        }

        query.Add($"includeMatchingSkill={includeMatchingSkill}");

        var uri =  $"api/employees?{string.Join("&", query)}";

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var employees = await response.Content.ReadFromJsonAsync<List<Employee>>(cancellationToken);
        return employees ?? [];
    }

    public async Task<Employee?> CreateEmployee(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var createRequest = new
        {
            employee.FirstName,
            employee.LastName,
            employee.Email
        };

        using var response = await httpClient.PostAsJsonAsync("api/employees", createRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Employee>(cancellationToken: cancellationToken);
    }
}
