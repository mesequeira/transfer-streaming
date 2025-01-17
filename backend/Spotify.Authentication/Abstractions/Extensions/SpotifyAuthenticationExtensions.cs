using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Streaming.SharedKernel;
using Streaming.SharedKernel.Options;

namespace Spotify.Authentication.Abstractions.Extensions;

public static class SpotifyAuthenticationExtensions
{
    public static void AddSpotifyAuthentication(
        this IServiceCollection services
    )
    {
        services
            .AddOptionsWithValidateOnStart<ProviderAuthenticationOptions>()
            .BindConfiguration(nameof(ProviderAuthenticationOptions))
            .ValidateOnStart();
        
        services.AddSingleton(SpotifyClientConfig.CreateDefault());
        services.AddScoped<SpotifyClientBuilder>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Providers.Spotify, policy =>
            {
                policy.AuthenticationSchemes.Add(Providers.Spotify);
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddSpotify(options =>
            {
                var provider = services.BuildServiceProvider();
                var spotifyOptions = provider.GetRequiredService<IOptions<ProviderAuthenticationOptions>>().Value;

                options.ClientId = spotifyOptions.ClientId;
                options.ClientSecret = spotifyOptions.ClientSecret;
                options.CallbackPath = spotifyOptions.CallbackPath;
                options.SaveTokens = true;
                var scopes = new List<string> {
                    Scopes.UserReadEmail, Scopes.UserReadPrivate
                };
                options.Scope.Add(string.Join(",", scopes));
            });
    }
}