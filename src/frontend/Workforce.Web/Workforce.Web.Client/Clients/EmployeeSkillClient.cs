namespace Workforce.Web.Client.Clients;

public class EmployeeSkillClient(HttpClient httpClient) : IEmployeeSkillClient
{
    public async Task AssignSkill(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default)
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
            employeeSkill.EmployeeId,
            employeeSkill.SkillId,
            employeeSkill.Proficiency,
            employeeSkill.YearsOfExperience
        };

        using var response = await httpClient.PostAsJsonAsync($"api/employee-skills", body, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}