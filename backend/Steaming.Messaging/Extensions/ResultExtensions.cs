using System.Net;
using Microsoft.AspNetCore.Http;
using Streaming.Result;

namespace Steaming.Messaging.Extensions;

public static class ResultExtensions
{
    public static async Task<IResult> HandleResultAsync(this Task<Result> resultTask)
    {
        var result = await resultTask;

        return result switch
        {
            { IsSuccess: true, HttpStatusCode: HttpStatusCode.OK } => Results.Ok(),
            { IsSuccess: false } => Problem(result),
            _ => Results.StatusCode((int)result.HttpStatusCode)
        };
    }

    public static async Task<IResult> HandleResultAsync<TResponse>(this Task<Result<TResponse>> resultTask)
    {
        var result = await resultTask;
        return result switch
        {
            { IsSuccess: true, HttpStatusCode: HttpStatusCode.OK } => Results.Ok(result.Value),
            Result { IsSuccess: false } failure => Problem(failure),
            _ => Results.StatusCode((int)result.HttpStatusCode)
        };
    }

    /// <summary>
    /// Format the result as a problem result with the appropriate status code and error details.
    /// </summary>
    /// <param name="result">The result to format as a problem result.</param>
    /// <returns>The problem result with the appropriate status code and error details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the result is successful or has no error.</exception>
    private static IResult Problem(Result result)
    {
        if (result is { IsSuccess: true })
        {
            throw new InvalidOperationException(
                "You are trying to create a problem result with a successful result."
            );
        }

        if (result.Error is null)
        {
            throw new InvalidOperationException(
                "You are trying to create a problem result without an error."
            );
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result)
        );
    }

    private static string GetTitle(Error error) =>
        error.Type switch
        {
            ErrorType.Validation => error.Code,
            ErrorType.Problem => error.Code,
            ErrorType.NotFound => error.Code,
            ErrorType.Conflict => error.Code,
            _ => "Server failure"
        };

    private static string GetDetail(Error error) =>
        error.Type switch
        {
            ErrorType.Validation => error.Description,
            ErrorType.Problem => error.Description,
            ErrorType.NotFound => error.Description,
            ErrorType.Conflict => error.Description,
            _ => "An unexpected error occurred"
        };

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

    private static Dictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?> { { "errors", validationError.Errors } };
    }
}