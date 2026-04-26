namespace kirfy_api.Presentation.Contracts.Responses;

public sealed record ArtistAlbumResponse(
    long Id,
    string Name,
    string Url,
    string? CoverArtUrl,
    DateOnly? ReleaseDate);
