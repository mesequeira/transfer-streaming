namespace Streaming.SharedKernel.Options;

public class ProviderAuthenticationOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string CallbackPath { get; set; }
    public required string RedirectUri { get; set; }
    public required string SuccessRedirect { get; set; }
    public required string FailureRedirect { get; set; }
    
}