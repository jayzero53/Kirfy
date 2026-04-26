using kirfy_api.Domain.Entities;

namespace kirfy_api.Domain.Interfaces;

public interface IArtistAlbumsCache
{
    Task<ArtistAlbums?> GetAsync(string artistName, CancellationToken cancellationToken = default);
    Task SetAsync(string artistName, ArtistAlbums albums, CancellationToken cancellationToken = default);
}
