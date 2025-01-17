using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;
using Streaming.SharedKernel;

namespace Spotify.Authentication.Playlists.Queries.GetPlaylists;

public sealed class GetPlaylistsQuery : PlaylistCurrentUsersRequest, IRequest<Result<Paging<FullPlaylist>>>;

internal sealed class GetPlaylistsQueryHandler(
    SpotifyClientBuilder builder
) : IRequestHandler<GetPlaylistsQuery, Result<Paging<FullPlaylist>>>
{
    public async Task<Result<Paging<FullPlaylist>>> Handle(GetPlaylistsQuery request, CancellationToken cancellationToken)
    {
        var client = await builder.BuildClient();

        var response = await client.Playlists.CurrentUsers(request, cancellationToken);

        return Result.Success(response);
    }
}

public class Playlists : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/spotify/playlists", GetPlaylistsAsync)
            .Produces<Paging<FullPlaylist>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Providers.Spotify)
            .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> GetPlaylistsAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(new GetPlaylistsQuery(), cancellationToken).HandleResultAsync();

        return result;
    }
}