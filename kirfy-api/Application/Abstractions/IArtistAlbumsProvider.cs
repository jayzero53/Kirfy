using kirfy_api.Domain.Entities;

namespace kirfy_api.Application.Abstractions;

public interface IArtistAlbumsProvider
{
    Task<ArtistAlbums?> GetByArtistNameAsync(string artistName, CancellationToken cancellationToken = default);
}
