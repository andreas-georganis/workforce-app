using Microsoft.AspNetCore.Diagnostics;
using Workforce.Domain.Exceptions;
using Workforce.Infrastructure;

namespace Workforce.API;

public class UniqueConstraintViolationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is UniqueConstraintViolationException uniqueConstraintViolationEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                error = uniqueConstraintViolationEx.Message,
                property = uniqueConstraintViolationEx.PropertyName
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true; // handled
        }

        if (exception is WorkforceDomainException domainException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                error = domainException.Message
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true; // handled
        }

        return false; // let other handlers (like the default) deal with it
    }
}