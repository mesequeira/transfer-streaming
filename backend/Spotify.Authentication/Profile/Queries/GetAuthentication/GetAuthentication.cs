using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Authentication.Abstractions.Builders;
using SpotifyAPI.Web;
using Steaming.Messaging.Extensions;
using Streaming.Result;
using Streaming.SharedKernel;

namespace Spotify.Authentication.Profile.Queries.GetAuthentication;

public sealed class GetAuthenticationQuery : IRequest<Result<PrivateUser>>;

internal sealed class GetAuthenticationQueryHandler(
    SpotifyClientBuilder builder,
    IHttpContextAccessor accessor
) : IRequestHandler<GetAuthenticationQuery, Result<PrivateUser>>
{
    public async Task<Result<PrivateUser>> Handle(GetAuthenticationQuery request,
        CancellationToken cancellationToken)
    {
        var context = accessor.HttpContext;
        
        var client = await builder.BuildClient();
        
        var user = await client.UserProfile.Current(cancellationToken);
        
        return Result.Success(user);
    }
}

public class Profile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/spotify/login", GetCallbackAuthenticationAsync)
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
        var result = await sender.Send(new GetAuthenticationQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Redirect("https://a795-181-229-41-219.ngrok-free.app");
        }

        return Results.InternalServerError();
    }
}