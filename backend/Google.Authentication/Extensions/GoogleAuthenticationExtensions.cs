using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Streaming.SharedKernel.Options;

namespace Google.Authentication.Extensions;

internal static class GoogleAuthenticationExtensions
{
    public static void AddGoogleAuthentication(
        this IServiceCollection services
    )
    {
        services
            .AddOptionsWithValidateOnStart<ProviderAuthenticationOptions>()
            .BindConfiguration(nameof(ProviderAuthenticationOptions))
            .ValidateOnStart();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddGoogle(options =>
            {
                var provider = services.BuildServiceProvider();
                var googleOptions = provider.GetRequiredService<IOptions<ProviderAuthenticationOptions>>().Value;

                options.ClientId = googleOptions.ClientId;
                options.ClientSecret = googleOptions.ClientSecret;
                options.CallbackPath = googleOptions.CallbackPath;
                options.SaveTokens = true;
            });

        services.AddAuthorization();
    }
}