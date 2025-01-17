using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;

namespace Spotify.Authentication.Playlists.Queries.GetTracks;

public record GetTracksQuery(
    string PlaylistId,
    PlaylistGetItemsRequest Parameters
) : IRequest<Result<Paging<PlaylistTrack<IPlayableItem>>>>;

internal sealed class GetTracksQueryHandler(
    SpotifyClientBuilder builder
) : IRequestHandler<GetTracksQuery, Result<Paging<PlaylistTrack<IPlayableItem>>>>
{
    public async Task<Result<Paging<PlaylistTrack<IPlayableItem>>>> Handle(GetTracksQuery request, CancellationToken cancellationToken)
    {
        var client = await builder.BuildClient();

        var response = await client.Playlists.GetItems(request.PlaylistId, request.Parameters, cancellationToken);

        return Result.Success(response);
    }
}

public class Playlists : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/spotify/playlists/{playlistId}/tracks", GetTracksAsync)
            .Produces<Paging<PlaylistTrack<IPlayableItem>>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> GetTracksAsync(
        [FromServices] ISender sender,
        [FromRoute] string playlistId,
        [FromQuery] string[]? fields,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        [FromQuery] string? market,
        [FromQuery] string? locale,
        CancellationToken cancellationToken
    )
    {
        var request = new PlaylistGetItemsRequest
        {
            Limit = limit,
            Offset = offset,
            Market = market,
            Locale = locale
        };

        if (fields is not null)
        {
            request.Fields.AddRange(fields);
        }
        
        var query = new GetTracksQuery(playlistId, request);
        
        var result = await sender.Send(query, cancellationToken).HandleResultAsync();

        return result;
    }
}