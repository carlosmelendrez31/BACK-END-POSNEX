using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Posnex.BLL.Businesses;

namespace Posnex.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var isValidationError = exception is BusinessValidationException;
        if (isValidationError)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else
        {
            logger.LogError(exception, "Unhandled API exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        var problem = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Title = isValidationError ? "Solicitud inválida" : "Error interno",
            Detail = isValidationError ? exception.Message : "Ocurrió un error. Comparte el identificador de seguimiento con soporte."
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }
}
