namespace Streaming.SharedKernel.Models;

public class ErrorResponse
{
    public ErrorMessage Error { get; set; } = default!;
}

public sealed record ErrorMessage(int Status, string Message);