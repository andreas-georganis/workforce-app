namespace Workforce.Web.Endpoints;

internal static class EmployeeSkillApi
{
    internal static void MapEmployeeSkillApi(this WebApplication app)
    {
        app.MapGroup("api/employee-skills")
        .WithTags("EmployeeSkills")
        .RequireAuthorization()
        .MapAuthorizedForwarder("{**catch-all}", "http://workforce-api");
    }
}