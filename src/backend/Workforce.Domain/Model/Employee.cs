using Workforce.Domain.Exceptions;

namespace Workforce.Domain.Model;

public sealed class Employee
{
    private readonly HashSet<EmployeeSkill> _skills;

    private Employee() => _skills = []; // For EF Core

    public Employee(EmployeeId id, FirstName firstName, LastName lastName, Email email, IEnumerable<EmployeeSkill> skills):this()
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _skills = [.. skills];
    }

    public EmployeeId Id { get; }
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public Email Email { get; }

    public IReadOnlyCollection<EmployeeSkill> Skills => _skills.AsReadOnly();

    public void AssignSkill(SkillId skillId, Proficiency proficiency, YearsOfExperience yearsOfExperience)
    {
        if (!_skills.Add(new EmployeeSkill(skillId, proficiency, yearsOfExperience)))
        {
            throw new WorkforceDomainException("Skill already assigned to employee.");
        }
    }
}