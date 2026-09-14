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

        var statusCode = exception switch
        {
            UniqueConstraintViolationException => StatusCodes.Status409Conflict,
            WorkforceDomainException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        ProblemDetails details = statusCode switch
        {
            StatusCodes.Status400BadRequest => new ValidationProblemDetails
            {
                Title = "An error occurred",
                Detail = exception.Message,
                Status = statusCode,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier,
                },
                Errors =
                {
                    ["error"] = new[] { exception.Message }
                }
            },
            _ => new ProblemDetails
            {
                Title = "An error occurred",
                Detail = "Please contact support",//exception.Message,
                Status = statusCode,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier,
                }
            }
        };

        //httpContext.Response.ContentType = "application/problem+json";

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = details,
        };

        return await _problemDetailsService.TryWriteAsync(context);
    }
}