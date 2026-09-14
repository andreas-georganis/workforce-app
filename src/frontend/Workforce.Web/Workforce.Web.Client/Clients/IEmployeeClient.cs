namespace Workforce.Web.Client.Clients;

public interface IEmployeeClient
{
    Task<IReadOnlyList<Employee>> GetEmployees(string? skillIdentifier = null, bool includeMatchingSkill = true, CancellationToken cancellationToken = default);

    Task<Employee?> CreateEmployee(Employee employee, CancellationToken cancellationToken = default);
}
