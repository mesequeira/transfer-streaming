using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Spotify.Authentication.Abstractions.Handlers;

internal sealed class SpotifyAuthorizationHandler(
    IHttpContextAccessor contextAccessor    
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = contextAccessor.HttpContext;

        if (context is null)
        {
            return await base.SendAsync(request, cancellationToken);    
        }

        var accessToken = await context.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return await base.SendAsync(request, cancellationToken);
        }
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        return await base.SendAsync(request, cancellationToken);
    }
}