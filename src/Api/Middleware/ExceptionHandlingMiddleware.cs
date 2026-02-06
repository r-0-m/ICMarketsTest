using System.Net.Mime;
using ICMarketsTest.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogWarning(ex, "External service failure: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Unexpected server error.");
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Invalid request.",
                StatusCodes.Status503ServiceUnavailable => "External service unavailable.",
                _ => "Server error."
            },
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        return context.Response.WriteAsJsonAsync(problem);
    }
}
