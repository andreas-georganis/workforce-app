using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;
using Workforce.Infrastructure;

namespace WorkForce.API.Endpoints;

public static class EmployeeApi
{
    public static RouteGroupBuilder MapEmployeeApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/employees").RequireAuthorization();

        group.WithTags("Employees");

        //TODO: IAsyncEnumerable, Paging
        group.MapGet("/", async Task<Ok<IEnumerable<Workforce.API.Contracts.Employee>>> (SkillName skill, WorkforceDbContext db,  CancellationToken cancellationToken) =>
        {
            var employees = await db.Employees.Where(e => e.Skills.Any(s => db.Skills.Where(sk => sk.Name == skill)
                      .Select(sk => sk.Id)
                      .Contains(s.SkillId)))
                      .Select(e=> new Workforce.API.Contracts.Employee{ Id = e.Id, FirstName = e.FirstName, LastName = e.LastName, Email = e.Email } )
                    .ToListAsync(cancellationToken);

            return TypedResults.Ok<IEnumerable<Workforce.API.Contracts.Employee>>(employees);
        });

        group.MapPut("/{employeeId}/skills/{skillId}", async Task<Results<NoContent, NotFound>> (EmployeeId employeeId, SkillId skillId, Workforce.API.Contracts.EmployeeSkill employeeSkill, WorkforceDbContext db, CancellationToken cancellationToken) =>
        {
            if (await db.Employees.FindAsync([employeeId], cancellationToken: cancellationToken) is not {} employee)
            {
                return TypedResults.NotFound();
            }

            if (await db.Skills.FindAsync([skillId], cancellationToken) is not {} skill)
            {
                return TypedResults.NotFound();
            }

            employee.AssignSkill(skill.Id, employeeSkill.Proficiency, employeeSkill.YearsOfExperience);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });

        return group;
    }
}
