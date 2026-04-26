using System.Collections.Concurrent;
using kirfy_api.Domain.Entities;
using kirfy_api.Domain.Interfaces;

namespace kirfy_api.Infrastructure.Repositories;

public sealed class InMemoryArtistAlbumsCache : IArtistAlbumsCache
{
    private readonly ConcurrentDictionary<string, ArtistAlbums> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    public Task<ArtistAlbums?> GetAsync(string artistName, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(artistName, out var albums);
        return Task.FromResult(albums);
    }

    public Task SetAsync(string artistName, ArtistAlbums albums, CancellationToken cancellationToken = default)
    {
        _cache[artistName] = albums;
        return Task.CompletedTask;
    }
}
