using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Streaming.SharedKernel.Extensions;

public static class RateLimitExtensions
{
    public static void AddSharedRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", config =>
            {
                config.PermitLimit = 10; // Número máximo de solicitudes permitidas
                config.Window = TimeSpan.FromSeconds(10); // Ventana de tiempo
            });
        });
    }
}