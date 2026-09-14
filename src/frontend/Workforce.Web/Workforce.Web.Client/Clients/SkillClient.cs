using System.Net.Http.Json;

namespace Workforce.Web.Client.Clients;


public sealed class SkillClient(HttpClient httpClient) : ISkillClient
{
    public async Task<IReadOnlyList<Skill>> GetSkills(CancellationToken cancellationToken = default)
    {
        var skills = await httpClient.GetFromJsonAsync<List<Skill>>("api/skills", cancellationToken);
        return skills ?? [];
    }

    public async Task<Skill?> CreateSkill(Skill skill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(skill);

        using var response = await httpClient.PostAsJsonAsync("api/skills", new { skill.Name }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var createdSkill = await response.Content.ReadFromJsonAsync<Skill>(cancellationToken);


        return createdSkill;
    }
}
