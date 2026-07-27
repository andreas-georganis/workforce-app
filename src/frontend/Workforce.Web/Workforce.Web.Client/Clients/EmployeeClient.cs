using System.Net.Http.Json;

namespace Workforce.Web.Client.Clients;

public sealed class EmployeeClient(HttpClient httpClient) : IEmployeeClient
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
            ? "api/employees"
            : $"api/employees?{string.Join("&", query)}";

        var employees = await httpClient.GetFromJsonAsync<List<Employee>>(uri, cancellationToken);
        return employees ?? [];
    }

    public async Task AssignSkillAsync(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employeeSkill);

        var body = new
        {
            employeeSkill.Proficiency,
            employeeSkill.YearsOfExperience
        };

        using var response = await httpClient.PutAsJsonAsync($"api/employees/{employeeSkill.EmployeeId}/skills/{employeeSkill.SkillId}", body, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
