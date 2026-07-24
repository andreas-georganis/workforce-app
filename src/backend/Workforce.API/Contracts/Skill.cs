using System.ComponentModel.DataAnnotations;
using Workforce.Domain.Model;

namespace Workforce.API.Contracts;

public sealed class Skill
{
    public SkillId Id { get; init; } = SkillId.New();

    [Required]
    public required SkillName Name { get; init; }
}
