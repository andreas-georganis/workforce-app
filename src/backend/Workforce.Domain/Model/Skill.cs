namespace Workforce.Domain.Model;

public sealed class Skill
{
    private Skill(){}
    public Skill(SkillId id, SkillName name)
    {
        Id = id;
        Name = name;
    }

    public SkillId Id { get; }
    public SkillName Name { get; }
}