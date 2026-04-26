using kirfy_api.Shared.Exceptions;

namespace kirfy_api.Presentation.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ResourceNotFoundException ex)
        {
            logger.LogWarning(ex, "Requested resource was not found.");
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ExternalServiceException ex)
        {
            logger.LogError(ex, "External service call failed.");
            await WriteProblemAsync(context, StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception.");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string detail)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new
        {
            Status = statusCode,
            Detail = detail
        });
    }
}
