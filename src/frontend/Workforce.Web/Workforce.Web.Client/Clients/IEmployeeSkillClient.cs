namespace Workforce.Web.Client.Clients;

public interface IEmployeeSkillClient
{
    Task AssignSkill(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default);
}