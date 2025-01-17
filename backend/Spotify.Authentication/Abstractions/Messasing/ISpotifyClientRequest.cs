using MediatR;
using SpotifyAPI.Web;
using Streaming.Result;

namespace Spotify.Authentication.Abstractions.Messasing;

public interface IBaseSpotifyClientRequest 
{
    SpotifyClient? Client { get; set; }
}

public interface ISpotifyClientRequest : IRequest<Result>, IBaseSpotifyClientRequest;

public interface ISpotifyClientRequest<TResponse> : IRequest<Result<TResponse>>, IBaseSpotifyClientRequest; 