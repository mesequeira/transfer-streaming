namespace Streaming.SharedKernel.Options;

public class ProviderAuthenticationOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string CallbackPath { get; set; }
    public required string RedirectUri { get; set; }
    public required string SuccessRedirectUri { get; set; }
    public required string FailureRedirectUri { get; set; }
    
}