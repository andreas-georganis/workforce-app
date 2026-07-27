using System.Net.Http.Json;
using Workforce.Web.Client.Clients;

namespace Workforce.Web.Clients;


public sealed class ServerSkillClient(HttpClient httpClient) : ISkillClient
{
    public async Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("skills", cancellationToken);
        response.EnsureSuccessStatusCode();

        var skills = await response.Content.ReadFromJsonAsync<List<Skill>>(cancellationToken);
        return skills ?? [];
    }

    public async Task<Skill?> CreateSkillAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(skill);

        using var response = await httpClient.PostAsJsonAsync("skills", new { skill.Name }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var createdSkill = await response.Content.ReadFromJsonAsync<Skill>(cancellationToken);


        return createdSkill;
    }
}
