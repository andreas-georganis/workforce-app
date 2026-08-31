using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;
using Workforce.Infrastructure;

namespace WorkForce.API.Endpoints;

public static class EmployeeApi
{
    public static RouteGroupBuilder MapEmployeeApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/employees");//.RequireAuthorization();

        group.WithTags("Employees");

        group.MapPost("/", async Task<Created<Workforce.API.Contracts.Employee>> (WorkforceDbContext db, Workforce.API.Contracts.Employee newEmployee, CancellationToken cancellationToken) =>
        {
            var employee = new Workforce.Domain.Model.Employee(newEmployee.Id, newEmployee.FirstName, newEmployee.LastName, newEmployee.Email, []);

            await db.Employees.AddAsync(employee, cancellationToken);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/employees/{employee.Id.Value}",
                new Workforce.API.Contracts.Employee
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email
                });
        });

        //TODO: IAsyncEnumerable, Paging
        group.MapGet("/", async Task<Ok<IEnumerable<Workforce.API.Contracts.Employee>>> (Workforce.API.Contracts.SkillIdentifier? skill, WorkforceDbContext db, CancellationToken cancellationToken, bool includeMatchingSkill = true) =>
        {
            IQueryable<Employee> query = db.Employees;

            if (skill is not null)
            {
                query = skill.Value.Id is not null
                    ? includeMatchingSkill
                        ? query.Where(e => e.Skills.Any(es => es.SkillId == skill.Value.Id.Value))
                        : query.Where(e => e.Skills.All(es => es.SkillId != skill.Value.Id.Value))
                    : includeMatchingSkill
                        ? query.Where(e => e.Skills.Any(es => db.Skills.Any(sk => sk.Id == es.SkillId && sk.Name == skill.Value.Name)))
                        : query.Where(e => e.Skills.All(es => !db.Skills.Any(sk => sk.Id == es.SkillId && sk.Name == skill.Value.Name)));
            }

            var employees = await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new Workforce.API.Contracts.Employee
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email
                })
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
