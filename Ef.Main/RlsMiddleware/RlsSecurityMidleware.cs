using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ef.Main
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;

        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, RlsSecurityContext securityContext)
        {
            securityContext.Owner = context.Request.Headers["X-Owner"];
            _logger.LogInformation($"Found Owner [{securityContext.Owner}]");
            await _next.Invoke(context);
        }
    }

    public static class SecurityMiddlewareAppBuilder
    {
        public static void UseRlsSecurity(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SecurityMiddleware>();
        }

        public static void AddRlsSecurity(this IServiceCollection builder)
        {
            builder.AddScoped<RlsSecurityContext>();
        }
    }
}