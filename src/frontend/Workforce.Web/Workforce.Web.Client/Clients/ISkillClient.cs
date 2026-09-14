namespace Workforce.Web.Client.Clients;


public interface ISkillClient
{
    Task<IReadOnlyList<Skill>> GetSkills(CancellationToken cancellationToken = default);

    Task<Skill?> CreateSkill(Skill skill, CancellationToken cancellationToken = default);
}
