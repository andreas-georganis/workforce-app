using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;
using Workforce.Infrastructure;

namespace Workforce.API.Endpoints;

public sealed class EmployeeApiBuilder(IVersionedEndpointRouteBuilder builder) : VersionedApiBuilder(builder);

public static class EmployeeApi
{
    extension(IEndpointRouteBuilder app)
    {
        public EmployeeApiBuilder MapEmployeeApi()
            => new (app.NewVersionedApi("Employees"));

    }

    extension (EmployeeApiBuilder apiBuilder)
    {
        public VersionedApiBuilder<Contracts.Employee> ToV1()
        {
            var employees = new VersionedApiBuilder<Contracts.Employee>(apiBuilder.Endpoints);

            var builder = employees.Endpoints;

            var group = employees.Endpoints.MapGroup("/api/employees").RequireAuthorization();

            group.MapPost("/", V1.Post)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            //.ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status409Conflict);

            //TODO: IAsyncEnumerable, Paging
            group.MapGet("/", V1.Get);

            return new(builder);

        }
    }

    private static class V1
    {
        internal static async Task<Ok<IEnumerable<Workforce.API.Contracts.Employee>>> Get(Workforce.API.Contracts.SkillIdentifier? skill, WorkforceDbContext db, CancellationToken cancellationToken, bool includeMatchingSkill = true)
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
        }

        internal static async Task<Created<Workforce.API.Contracts.Employee>> Post(WorkforceDbContext db, Workforce.API.Contracts.Employee newEmployee, CancellationToken cancellationToken)
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
        }
    }

}
