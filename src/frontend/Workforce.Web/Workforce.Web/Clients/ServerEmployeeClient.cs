using System.Net.Http.Json;
using Workforce.Web.Client.Clients;

namespace Workforce.Web.Clients;

public sealed class ServerEmployeeClient(HttpClient httpClient) : IEmployeeClient
{
    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(string? skillIdentifier = null, bool includeMatchingSkill = true, CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(skillIdentifier))
        {
            query.Add($"skill={Uri.EscapeDataString(skillIdentifier)}");
        }

        query.Add($"includeMatchingSkill={includeMatchingSkill}");

        var uri = query.Count == 0
            ? "employees"
            : $"employees?{string.Join("&", query)}";

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var employees = await response.Content.ReadFromJsonAsync<List<Employee>>(cancellationToken);
        return employees ?? [];
    }

    public async Task AssignSkillAsync(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employeeSkill);

        if (employeeSkill.EmployeeId is null)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(employeeSkill));
        }

        if (employeeSkill.SkillId is null)
        {
            throw new ArgumentException("SkillId is required.", nameof(employeeSkill));
        }

        var body = new
        {
            employeeSkill.Proficiency,
            employeeSkill.YearsOfExperience
        };

        using var response = await httpClient.PutAsJsonAsync($"employees/{employeeSkill.EmployeeId}/skills/{employeeSkill.SkillId}", body, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Employee?> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var createRequest = new
        {
            employee.FirstName,
            employee.LastName,
            employee.Email
        };

        var response = await httpClient.PostAsJsonAsync("employees", createRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Employee>(cancellationToken: cancellationToken);
    }
}
