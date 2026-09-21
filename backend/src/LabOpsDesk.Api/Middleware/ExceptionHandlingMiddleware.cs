using System.Net;
using System.Text.Json;
using LabOpsDesk.Api.Contracts;
using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, ex);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (status, code, message) = exception switch
        {
            EntityNotFoundException notFound => (HttpStatusCode.NotFound, "not_found", notFound.Message),
            ConflictException conflict => (HttpStatusCode.Conflict, "conflict", conflict.Message),
            InvalidAssetTransitionException transition => (HttpStatusCode.Conflict, "invalid_transition", transition.Message),
            ArgumentException argument => (HttpStatusCode.BadRequest, "bad_request", argument.Message),
            InvalidOperationException invalid => (HttpStatusCode.BadRequest, "bad_request", invalid.Message),
            _ => (HttpStatusCode.InternalServerError, "server_error", "An unexpected error occurred.")
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = new ApiErrorResponse(code, message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
