using System.Diagnostics.CodeAnalysis;

namespace Workforce.Domain.Model;

public sealed class Skill
{
    [ExcludeFromCodeCoverage(Justification = "EF Core")]
    private Skill(){}
    public Skill(SkillId id, SkillName name)
    {
        Id = id;
        Name = name;
    }

    public SkillId Id { get; }
    public SkillName Name { get; }
}