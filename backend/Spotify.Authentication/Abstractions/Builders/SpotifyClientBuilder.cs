using Microsoft.AspNetCore.Authentication;
using SpotifyAPI.Web;
using Streaming.SharedKernel;

namespace Spotify.Authentication.Abstractions.Builders;

public class SpotifyClientBuilder(
  IHttpContextAccessor httpContextAccessor,
  SpotifyClientConfig spotifyClientConfig
)
{
  public async Task<SpotifyClient> BuildClient()
  {
    var context = httpContextAccessor.HttpContext;

    ArgumentNullException.ThrowIfNull(context);

    var accessToken = await context.GetTokenAsync(Providers.Spotify, "access_token");

    ArgumentNullException.ThrowIfNull(accessToken);

    return new SpotifyClient(spotifyClientConfig.WithToken(accessToken));
  }
}
