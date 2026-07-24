using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;
using Workforce.Infrastructure;

namespace Workforce.API.Endpoints;

public static class SkillApi
{
    public static RouteGroupBuilder MapSkillApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/skills").RequireAuthorization();

        group.WithTags("Skills");

        // paging
        group.MapGet("/", async (WorkforceDbContext db, CancellationToken cancellationToken) =>
        {
           return await db.Skills.Select(s=> new Contracts.Skill { Id = s.Id, Name = s.Name}).ToListAsync(cancellationToken);
        });

        group.MapGet("/{id}", async Task<Results<Ok<Workforce.API.Contracts.Skill>, NotFound>> (WorkforceDbContext db, SkillId id, CancellationToken cancellationToken) =>
        {
            return await db.Skills.FindAsync([id], cancellationToken) switch
            {
                Skill skill => TypedResults.Ok(new Contracts.Skill { Id = skill.Id, Name = skill.Name}),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<Workforce.API.Contracts.Skill>> (WorkforceDbContext db, Workforce.API.Contracts.Skill newSkill, CancellationToken cancellationToken) =>
        {
            var skill = new Domain.Model.Skill(newSkill.Id, newSkill.Name);

            await db.Skills.AddAsync(skill, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/skills/{skill.Id.Value}", new Contracts.Skill { Id = skill.Id, Name = skill.Name});
        });

        return group;
    }
}