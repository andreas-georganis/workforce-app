using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Workforce.Infrastructure;

namespace Workforce.API.Endpoints;

public sealed class EmployeeSkillApiBuilder(IVersionedEndpointRouteBuilder builder) : VersionedApiBuilder(builder);

public static class EmployeeSkillApi
{
	extension(IEndpointRouteBuilder app)
	{
		public EmployeeSkillApiBuilder MapEmployeeSkillApi()
			=> new(app.NewVersionedApi("Employee Skills"));
	}

	extension(EmployeeSkillApiBuilder apiBuilder)
	{
		public VersionedApiBuilder<Contracts.EmployeeSkill> ToV1()
		{
			var employeeSkills = new VersionedApiBuilder<Contracts.EmployeeSkill>(apiBuilder.Endpoints);

			var builder = employeeSkills.Endpoints;

			var group = employeeSkills.Endpoints.MapGroup("/api/employee-skills").RequireAuthorization();

            group.MapPost("/", V1.Post)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest);

			return new(builder);
		}
	}

    private static class V1
    {
        public static async Task<Results<NoContent, NotFound>> Post(Contracts.EmployeeSkill employeeSkill, WorkforceDbContext db, CancellationToken cancellationToken)
        {
            if (await db.Employees.FindAsync([employeeSkill.EmployeeId], cancellationToken: cancellationToken) is not {} employee)
            {
                return TypedResults.NotFound();
            }

            if (await db.Skills.FindAsync([employeeSkill.SkillId], cancellationToken: cancellationToken) is not {} skill)
            {
                return TypedResults.NotFound();
            }

            employee.AssignSkill(skill.Id, employeeSkill.Proficiency, employeeSkill.YearsOfExperience);

            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }
    }
}
