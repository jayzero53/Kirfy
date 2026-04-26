namespace kirfy_api.Presentation.Contracts.Responses;

public sealed record ArtistAlbumsResponse(
    long ArtistId,
    string ArtistName,
    IReadOnlyList<ArtistAlbumResponse> Albums);
