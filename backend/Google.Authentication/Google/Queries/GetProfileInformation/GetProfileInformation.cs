using System.Net;
using Carter;
using Google.Authentication.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Steaming.Messaging.Extensions;
using Streaming.Result;

namespace Google.Authentication.Google.Queries.GetProfileInformation;

public sealed class GetProfileInformationQuery : IRequest<Result<string>>;

internal sealed class GetProfileInformationQueryHandler(
    IHttpContextAccessor contextAccessor
) : IRequestHandler<GetProfileInformationQuery, Result<string>>
{
    public Task<Result<string>> Handle(GetProfileInformationQuery request,
        CancellationToken cancellationToken)
    {
        var context = contextAccessor.HttpContext;

        if (context is null)
        {
            return Task.FromResult(Result.Failure<string>(Errors.CouldNotAccessHttpContext,
                HttpStatusCode.InternalServerError));
        }

        var accessToken = context.Request.Cookies["access_token"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Task.FromResult(Result.Failure<string>(Errors.AccessTokenNotFound, HttpStatusCode.Unauthorized));
        }

        return Task.FromResult(Result.Success(accessToken));
    }
}

public class GetCallbackAuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/google/profile", GetCallbackAuthenticationAsync);
    }

    public static async Task<IResult> GetCallbackAuthenticationAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        return await sender.Send(new GetProfileInformationQuery(), cancellationToken).HandleResultAsync();
    }
}