using System.Net.Http.Json;
using Workforce.Web.Client.Clients;

namespace Workforce.Web.Clients;


public sealed class SkillClient(HttpClient httpClient) : ISkillClient
{
    public async Task<IReadOnlyList<Skill>> GetSkills(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/skills", cancellationToken);
        response.EnsureSuccessStatusCode();

        var skills = await response.Content.ReadFromJsonAsync<List<Skill>>(cancellationToken);
        return skills ?? [];
    }

    public async Task<Skill?> CreateSkill(Skill skill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(skill);

        using var response = await httpClient.PostAsJsonAsync("api/skills", new { skill.Name }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var createdSkill = await response.Content.ReadFromJsonAsync<Skill>(cancellationToken: cancellationToken);

        return createdSkill;
    }
}
