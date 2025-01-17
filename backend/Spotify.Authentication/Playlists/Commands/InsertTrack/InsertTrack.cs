using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;

namespace Spotify.Authentication.Playlists.Commands.InsertTrack;

public sealed record InsertTrack(
    string PlaylistId,
    int Position,
    List<string> Uris
) : IRequest<Result<SnapshotResponse>>;

internal sealed class InserTrackValidator : AbstractValidator<InsertTrack>
{
    public InserTrackValidator()
    {
        RuleFor(x => x.Position)
            .GreaterThan(0)
            .WithMessage("The position must be greater than 0");

        RuleFor(x => x.Uris)
            .NotEmpty()
            .WithMessage("The Uris must not be empty");
    }
}

internal sealed class InsertTrackHandler(
    SpotifyClientBuilder builder
) : IRequestHandler<InsertTrack, Result<SnapshotResponse>>
{
    public async Task<Result<SnapshotResponse>> Handle(InsertTrack request, CancellationToken cancellationToken)
    {
        var client = await builder.BuildClient();
        
        var parameter = request.Adapt<PlaylistAddItemsRequest>(); 
            
        var response = await client.Playlists.AddItems(request.PlaylistId, parameter, cancellationToken);

        return Result.Success(response);
    }
}

public class Playlists : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/spotify/playlists/tracks", InsertTrackAsync)
            .RequireAuthorization()
            .Produces<SnapshotResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> InsertTrackAsync(
        [FromServices] ISender sender,
        [FromBody] InsertTrack insertTrack,
        CancellationToken cancellationToken
    )
    {
        return await sender.Send(insertTrack, cancellationToken).HandleResultAsync();
    }
}