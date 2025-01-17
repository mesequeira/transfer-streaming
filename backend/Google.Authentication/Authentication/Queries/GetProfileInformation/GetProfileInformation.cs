// using System.Net;
// using System.Security.Claims;
// using Carter;
// using Google.Authentication.Models;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Steaming.Messaging.Extensions;
// using Streaming.Result;
// using Streaming.SharedKernel.Models;
//
// namespace Google.Authentication.Authentication.Queries.GetProfileInformation;
//
// public sealed class GetProfileInformationQuery : IRequest<Result<IEnumerable<ClaimsResult>>>;
//
// internal sealed class GetProfileInformationQueryHandler(
//     IHttpContextAccessor contextAccessor
// ) : IRequestHandler<GetProfileInformationQuery, Result<IEnumerable<ClaimsResult>>>
// {
//     public Task<Result<IEnumerable<ClaimsResult>>> Handle(GetProfileInformationQuery request,
//         CancellationToken cancellationToken)
//     {
//         var context = contextAccessor.HttpContext;
//
//         if (context is null)
//         {
//             return Task.FromResult(Result.Failure<IEnumerable<ClaimsResult>>(Errors.CouldNotAccessHttpContext,
//                 HttpStatusCode.InternalServerError));
//         }
//
//         var accessToken = context.Request.Cookies["access_token"];
//
//         if (string.IsNullOrEmpty(accessToken))
//         {
//             return Task.FromResult(Result.Failure<IEnumerable<ClaimsResult>>(Errors.AccessTokenNotFound, HttpStatusCode.Unauthorized));
//         }
//
//         var claims = context.User.Claims.Select(claim => new ClaimsResult(claim.Type, claim.Value));
//
//         return Task.FromResult(Result.Success(claims));
//     }
// }
//
// public class GetCallbackAuthenticationEndpoint : ICarterModule
// {
//     public void AddRoutes(IEndpointRouteBuilder app)
//     {
//         app.MapGet("/api/google/profile", GetCallbackAuthenticationAsync)
//             .RequireAuthorization()
//             .RequireRateLimiting("fixed");
//     }
//
//     public static async Task<IResult> GetCallbackAuthenticationAsync(
//         [FromServices] ISender sender,
//         CancellationToken cancellationToken
//     )
//     {
//         var result = await sender.Send(new GetProfileInformationQuery(), cancellationToken).HandleResultAsync();
//
//         return result;
//     }
// }