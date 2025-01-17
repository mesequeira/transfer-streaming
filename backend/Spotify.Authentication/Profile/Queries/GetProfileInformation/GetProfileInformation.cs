using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;
using Streaming.SharedKernel;

namespace Spotify.Authentication.Profile.Queries.GetProfileInformation;

public sealed class GetProfileInformationQuery : IRequest<Result<PrivateUser>>;

internal sealed class GetProfileInformationQueryHandler(
    SpotifyClientBuilder builder
) : IRequestHandler<GetProfileInformationQuery, Result<PrivateUser>>
{
    public async Task<Result<PrivateUser>> Handle(GetProfileInformationQuery request,
        CancellationToken cancellationToken)
    {
        var client = await builder.BuildClient();
        
        var user = await client.UserProfile.Current(cancellationToken);
        
        return Result.Success(user);
    }
}

public class Profile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/spotify/api/profile", GetCallbackAuthenticationAsync)
            .Produces<PrivateUser>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Providers.Spotify)
            .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> GetCallbackAuthenticationAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        return await sender.Send(new GetProfileInformationQuery(), cancellationToken).HandleResultAsync();
    }
}