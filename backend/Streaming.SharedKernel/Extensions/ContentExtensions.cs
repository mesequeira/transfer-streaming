using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Streaming.SharedKernel.Models;
using ResultType = Streaming.Result.Result;

namespace Streaming.SharedKernel.Extensions;

public static class ContentExtensions
{
    public static StringContent ToJsonContent<T>(this T content)
    {
        var serialized = JsonSerializer.Serialize(content);
        
        return new StringContent(serialized, Encoding.UTF8, "application/json");
    }

    public static async Task<Result<T>> HandleResponseAsync<T>(this HttpContent content, CancellationToken cancellationToken)
        where T : ErrorResponse
    {
        var response = await content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        
        if(response?.Error != null)
        {
            var error = new Error(response.Error.Status.ToString(), response.Error.Message, ErrorType.Failure); 
            
            return ResultType.Failure<T>(error, HttpStatusCode.BadRequest);
        }
        
        return ResultType.Success(response)!;
    }
}