namespace kirfy_api.Infrastructure.Services.Genius;

public sealed class GeniusApiOptions
{
    public const string SectionName = "GeniusAPI";

    public string BaseUrl { get; init; } = "https://api.genius.com";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string ClientAccessToken { get; init; } = string.Empty;
}
