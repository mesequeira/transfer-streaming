using SpotifyAPI.Web;

namespace Streaming.SharedKernel;

public static class ProvidersScopes
{
    public static IEnumerable<string> Spotify = new List<string>
    {
        Scopes.UgcImageUpload,
        Scopes.UserReadPlaybackState,
        Scopes.UserModifyPlaybackState,
        Scopes.UserReadCurrentlyPlaying,
        Scopes.Streaming,
        Scopes.AppRemoteControl,
        Scopes.UserReadEmail,
        Scopes.UserReadPrivate,
        Scopes.PlaylistReadCollaborative,
        Scopes.PlaylistReadPrivate,
        Scopes.PlaylistModifyPublic,
        Scopes.PlaylistModifyPrivate,
        Scopes.UserLibraryModify,
        Scopes.UserLibraryRead,
        Scopes.UserTopRead,
        Scopes.UserReadPlaybackPosition,
        Scopes.UserReadRecentlyPlayed,
        Scopes.UserFollowRead,
        Scopes.UserFollowModify
    };

}