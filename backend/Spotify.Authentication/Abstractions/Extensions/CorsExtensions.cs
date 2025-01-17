using Microsoft.Extensions.Options;
using Streaming.SharedKernel.Options;

namespace Spotify.Authentication.Abstractions.Extensions;

public static class CorsExtensions
{
    public static void AddWebsiteCors(
        this IServiceCollection services
    )
    {
        services
            .AddOptionsWithValidateOnStart<CorsOptions>()
            .BindConfiguration(nameof(CorsOptions))
            .ValidateOnStart();
        
        services.AddCors(options =>
        {
            var provider = services.BuildServiceProvider();
            var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
            
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.WithOrigins(corsOptions.Url)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}