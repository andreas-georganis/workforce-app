namespace Workforce.Web.Client.Clients;

public interface IEmployeeClient
{
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(string? skillIdentifier = null, bool includeMatchingSkill = true, CancellationToken cancellationToken = default);

    Task<Employee?> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);

    Task AssignSkillAsync(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default);
}
