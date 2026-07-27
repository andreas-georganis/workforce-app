using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace Workforce.Web.Endpoints;

internal static class EmployeeApi
{
    public static void MapEmployeeApi(this WebApplication app)
    {
        var group = app.MapGroup("api/employees")
            .WithTags("Employees")
            .RequireAuthorization()
            .MapAuthorizedForwarder("{**catch-all}", "http://workforce-api");
    }
}