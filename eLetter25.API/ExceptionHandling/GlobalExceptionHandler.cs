using eLetter25.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.ExceptionHandling;

/// <summary>
/// Translates unhandled exceptions into RFC 7807 ProblemDetails responses.
/// Centralising this here keeps controllers and handlers free of exception-mapping concerns.
/// </summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapToStatusCode(exception);
        var traceId = httpContext.TraceIdentifier;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                "Unhandled exception [{ExceptionType}] during {Method} {Path}. TraceId: {TraceId}",
                exception.GetType().FullName,
                httpContext.Request.Method,
                httpContext.Request.Path,
                traceId);

            logger.LogDebug(
                exception,
                "Full exception details for TraceId {TraceId}",
                traceId);
        }
        else
        {
            logger.LogWarning(
                "Domain validation failure [{ExceptionType}] during {Method} {Path}: {Message}",
                exception.GetType().Name,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = BuildProblemDetails(exception, statusCode, title, traceId);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails BuildProblemDetails(
        Exception exception,
        int statusCode,
        string title,
        string traceId)
    {
        var isClientError = statusCode < StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            // Expose the message only for client errors – never leak internal details for 5xx.
            Detail = isClientError ? exception.Message : null
        };

        // Include the TraceId so clients can report it to support for correlation with server logs.
        problemDetails.Extensions["traceId"] = traceId;

        if (exception is not ExceptionBase domainException)
        {
            return problemDetails;
        }

        problemDetails.Extensions["errorCode"] = domainException.ErrorCode;

        if (!string.IsNullOrWhiteSpace(domainException.Details))
        {
            problemDetails.Extensions["details"] = domainException.Details;
        }

        return problemDetails;
    }

    private static (int StatusCode, string Title) MapToStatusCode(Exception exception) =>
        exception switch
        {
            DomainValidationException => (StatusCodes.Status400BadRequest, "Domain Validation Error"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Request"),
            ExceptionBase => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };
}