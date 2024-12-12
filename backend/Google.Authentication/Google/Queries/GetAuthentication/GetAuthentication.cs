using System.Net;
using Carter;
using Google.Authentication.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Steaming.Messaging.Extensions;
using Streaming.Result;
using Streaming.SharedKernel;
using Streaming.SharedKernel.Options;

namespace Google.Authentication.Google.Queries.GetAuthentication;

public sealed class GetAuthenticationQuery : IRequest<Result>;

internal sealed class GetAuthenticationQueryHandler(
    IHttpContextAccessor contextAccessor,
    IOptions<ProviderAuthenticationOptions> options
) : IRequestHandler<GetAuthenticationQuery, Result>
{
    public async Task<Result> Handle(GetAuthenticationQuery request, CancellationToken cancellationToken)
    {
        var context = contextAccessor.HttpContext;

        if (context is null)
        {
            return Result.Failure(Errors.CouldNotAccessHttpContext, HttpStatusCode.InternalServerError);
        }

        await context.ChallengeAsync(Providers.Google, new AuthenticationProperties
        {
            RedirectUri = options.Value.RedirectUri
        });

        return Result.Success(HttpStatusCode.NoContent);
    }
}

public class GetAuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/google/login", GetAuthenticationAsync);
    }

    public static async Task GetAuthenticationAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        await sender.Send(new GetAuthenticationQuery(), cancellationToken);
    }
}