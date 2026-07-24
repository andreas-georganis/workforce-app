namespace Workforce.Domain.Model;

public sealed class EmployeeSkill : IEquatable<EmployeeSkill>
{
    private EmployeeSkill() { } // For EF Core
    public EmployeeSkill(SkillId skillId, Proficiency proficiency, YearsOfExperience yearsOfExperience)
    {
        SkillId = skillId;
        Proficiency = proficiency;
        YearsOfExperience = yearsOfExperience;
    }

    public SkillId SkillId { get; }
    public Proficiency Proficiency { get; }
    public YearsOfExperience YearsOfExperience { get; }

    public bool Equals(EmployeeSkill? other)
    {
        if (other is null)
        {
            return false;
        }

        return SkillId.Equals(other.SkillId);
    }
}