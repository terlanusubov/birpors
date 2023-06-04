using Microsoft.AspNetCore.Builder;

namespace Birpors.API.Middlewares
{
    public static class RequestMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomLogger>();
        }
    }
}
