namespace kirfy_api.Application.Features.Artists;

public sealed record ArtistProfileSnapshot(
    long Id,
    string Name,
    string? ImageUrl);
