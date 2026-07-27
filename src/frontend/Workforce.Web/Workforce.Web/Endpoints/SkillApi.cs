using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace Workforce.Web.Endpoints;

public static class SkillApi
{
    public static void MapSkillApi(this WebApplication app)
    {
        var group = app.MapGroup("api/skills")
            .WithTags("Skills")
            .RequireAuthorization();

        group.MapAuthorizedForwarder("{**catch-all}", "http://workforce-api");
    }
}