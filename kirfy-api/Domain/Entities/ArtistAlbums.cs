namespace kirfy_api.Domain.Entities;

public sealed record ArtistAlbums(
    long ArtistId,
    string ArtistName,
    IReadOnlyList<ArtistAlbum> Albums);
