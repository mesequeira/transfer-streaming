using System.Net;
using Carter;
using Google.Authentication.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Streaming.Result;
using Streaming.SharedKernel;
using Streaming.SharedKernel.Options;

namespace Google.Authentication.Authentication.Queries.GetCallbackAuthentication;

public sealed class GetCallbackAuthenticationQuery : IRequest<Result<string>>;

internal sealed class GetCallbackAuthenticationQueryHandler(
    IHttpContextAccessor contextAccessor,
    IOptions<ProviderAuthenticationOptions> options
) : IRequestHandler<GetCallbackAuthenticationQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetCallbackAuthenticationQuery request,
        CancellationToken cancellationToken)
    {
        var context = contextAccessor.HttpContext;

        if (context is null)
        {
            return Result.Failure<string>(Errors.CouldNotAccessHttpContext, HttpStatusCode.InternalServerError);
        }

        var result = await context.AuthenticateAsync(Providers.Google);

        // Something went wrong with the authentication because the user is not logged in or the session has expired
        if (!result.Succeeded)
        {
            return Result.Failure<string>(Errors.AuthenticationFailed, HttpStatusCode.Unauthorized);
        }

        // The user has successfully logged in
        var token = result.Properties.GetTokenValue("access_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Success(options.Value.FailureRedirectUri);
        }

        context.Response.Cookies.Append("access_token", token, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        return Result.Success(options.Value.SuccessRedirectUri);
    }
}

public class GetCallbackAuthenticationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/signin-google", GetCallbackAuthenticationAsync);
    }

    public static async Task<IResult> GetCallbackAuthenticationAsync(
        [FromServices] ISender sender,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(new GetCallbackAuthenticationQuery(), cancellationToken);

        return result.IsSuccess ? Results.Redirect(result.Value) : Results.StatusCode((int)result.HttpStatusCode);
    }
    
    /// <summary>
    /// When the provider can't authenticate the user because the session has expired or the user is not logged in,
    /// it will redirect to this URL so we redirect to the login page where will return a 401.
    /// </summary>
    /// <returns>A redirect to the login page.</returns>
    public static Task<IResult> GetLoginAsync()
    {
        return Task.FromResult(Results.Redirect($"/signin-google"));
    }
}