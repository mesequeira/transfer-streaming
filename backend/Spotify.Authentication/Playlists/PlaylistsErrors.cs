using Streaming.Result;

namespace Spotify.Authentication.Playlists;

public static class PlaylistsErrors
{
    public static readonly Error CouldNotDeserializeResponse = Error.Conflict(
        nameof(CouldNotDeserializeResponse),
        "Something goes wrong trying to deserialize the response"
    );
}