using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Workforce.Domain.Exceptions;
using Workforce.Infrastructure;

namespace Workforce.API;

public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<DefaultExceptionHandler> _logger;

    public DefaultExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<DefaultExceptionHandler> logger)
    {
        this._problemDetailsService = problemDetailsService;
        this._logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "{message}", exception.Message);

        var details = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Status = exception switch
            {
                UniqueConstraintViolationException => StatusCodes.Status409Conflict,
                WorkforceDomainException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            },
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
            }
        };

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = details,
        };

        return await _problemDetailsService.TryWriteAsync(context);


        // if (exception is UniqueConstraintViolationException uniqueConstraintViolationEx)
        // {
        //     httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        //     httpContext.Response.ContentType = "application/json";

        //     var response = new
        //     {
        //         error = uniqueConstraintViolationEx.Message,
        //         property = uniqueConstraintViolationEx.PropertyName
        //     };

        //     await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        //     return true; // handled
        // }

        // if (exception is WorkforceDomainException domainException)
        // {
        //     httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        //     httpContext.Response.ContentType = "application/json";

        //     var response = new
        //     {
        //         error = domainException.Message
        //     };

        //     await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        //     return true; // handled
        // }

        // return false; // let other handlers (like the default) deal with it
    }
}