namespace Workforce.Web.Client.Clients;


public interface ISkillClient
{
    Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default);

    Task<Skill?> CreateSkillAsync(Skill skill, CancellationToken cancellationToken = default);
}
