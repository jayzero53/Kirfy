namespace kirfy_api.Domain.Entities;

public sealed record ArtistAlbum(
    long Id,
    string Name,
    string Url,
    string? CoverArtUrl,
    DateOnly? ReleaseDate);
