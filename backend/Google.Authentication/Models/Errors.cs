using Streaming.Result;

namespace Google.Authentication.Models;

public static class Errors
{
    public static Error CouldNotAccessHttpContext = Error.Conflict(
        nameof(CouldNotAccessHttpContext),
        "Could not access the HttpContext"
    );
    
    public static Error AuthenticationFailed = Error.Conflict(
        nameof(AuthenticationFailed),
        "The authentication with the provider failed"
    );
    
    public static Error AccessTokenNotFound = Error.Conflict(
        nameof(AccessTokenNotFound),
        "The access token was not found in the authentication result"
    );
}