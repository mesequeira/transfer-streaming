using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;
using Streaming.SharedKernel;

namespace Spotify.Authentication.Search.GetSearch;

public sealed record GetSearchQuery(
    SearchRequest Parameters    
) : IRequest<Result<SearchResponse>>;

internal sealed class GetTrackQueryHandler(
    SpotifyClientBuilder builder
) : IRequestHandler<GetSearchQuery, Result<SearchResponse>>
{
    public async Task<Result<SearchResponse>> Handle(GetSearchQuery request, CancellationToken cancellationToken)
    {
        var client = await builder.BuildClient();

        var response = await client.Search.Item(request.Parameters, cancellationToken);

        return Result.Success(response);
    }
}

public class Search : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/spotify/search", GetPlaylistsAsync)
            .RequireAuthorization(Providers.Spotify)
            .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> GetPlaylistsAsync(
        [FromServices] ISender sender,
        [FromQuery] string q,
        [FromQuery] SearchRequest.Types type,
        [FromQuery] string? market,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        [FromQuery] SearchRequest.IncludeExternals? includeExternal,
        CancellationToken cancellationToken
    )
    {
        var parameters = new SearchRequest(type, q)
        {
            Market = market,
            Limit = limit,
            Offset = offset,
            IncludeExternal = includeExternal
        };
        return await sender.Send(new GetSearchQuery(parameters), cancellationToken)
            .HandleResultAsync();
    }
}