using kirfy_api.Presentation.Middleware;

namespace kirfy_api.Shared.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseKirfyExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
