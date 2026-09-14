using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;
using Workforce.Infrastructure;

namespace Workforce.API.Endpoints;

public sealed class SkillApiBuilder(IVersionedEndpointRouteBuilder builder) : VersionedApiBuilder(builder);

public static class SkillApi
{
    extension(IEndpointRouteBuilder app)
    {
        public SkillApiBuilder MapSkillApi()
            => new (app.NewVersionedApi("Skills"));
    }

    extension (SkillApiBuilder apiBuilder)
    {
        public VersionedApiBuilder<Contracts.Skill> ToV1()
        {
            var skills = new VersionedApiBuilder<Contracts.Skill>(apiBuilder.Endpoints);

            var builder = skills.Endpoints;

            var group = skills.Endpoints.MapGroup("/api/skills").RequireAuthorization();

            group.MapGet("/", V1.Get);

            group.MapGet("/{id}", V1.GetById);

            group.MapPost("/", V1.Post)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

            return new(builder);
        }
    }

    public static class V1
    {
        internal static async Task<Ok<IEnumerable<Contracts.Skill>>> Get(WorkforceDbContext db, CancellationToken cancellationToken)
        {
            var skills = await db.Skills
                .OrderBy(s => s.Name)
                .Select(s => new Contracts.Skill { Id = s.Id, Name = s.Name })
                .ToListAsync(cancellationToken);

            return TypedResults.Ok<IEnumerable<Contracts.Skill>>(skills);
        }

        internal static async Task<Results<Ok<Contracts.Skill>, NotFound>> GetById(WorkforceDbContext db, SkillId id, CancellationToken cancellationToken)
        {
            return await db.Skills.FindAsync([id], cancellationToken) switch
            {
                Skill skill => TypedResults.Ok(new Contracts.Skill { Id = skill.Id, Name = skill.Name }),
                _ => TypedResults.NotFound()
            };
        }

        internal static async Task<Created<Contracts.Skill>> Post(WorkforceDbContext db, Contracts.Skill newSkill, CancellationToken cancellationToken)
        {
            var skill = new Domain.Model.Skill(newSkill.Id, newSkill.Name);

            await db.Skills.AddAsync(skill, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/skills/{skill.Id.Value}", new Contracts.Skill { Id = skill.Id, Name = skill.Name });
        }
    }
}