using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using kirfy_api.Application.Abstractions;
using kirfy_api.Domain.Entities;
using Microsoft.Extensions.Options;

namespace kirfy_api.Infrastructure.Services.Genius;

public sealed class GeniusArtistAlbumsProvider : IArtistAlbumsProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public GeniusArtistAlbumsProvider(HttpClient httpClient, IOptions<GeniusApiOptions> options)
    {
        _httpClient = httpClient;
        var configuredOptions = options.Value;

        if (string.IsNullOrWhiteSpace(configuredOptions.ClientAccessToken))
        {
            throw new InvalidOperationException("Missing Genius API token. Configure GeniusAPI:clientAccessToken.");
        }
    }

    public async Task<ArtistAlbums?> GetByArtistNameAsync(string artistName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(artistName))
        {
            throw new ArgumentException("Artist name is required.", nameof(artistName));
        }

        var artist = await ResolveArtistAsync(artistName, cancellationToken);
        if (artist is null)
        {
            return null;
        }

        var albums = new List<ArtistAlbum>();
        var page = 1;

        while (true)
        {
            var endpoint = $"/artists/{artist.Id}/albums?per_page=50&page={page}";
            var payload = await GetAndDeserializeAsync<GeniusArtistAlbumsEnvelope>(endpoint, cancellationToken);
            var response = payload.Response;

            if (response?.Albums is { Count: > 0 })
            {
                albums.AddRange(response.Albums.Select(MapAlbum));
            }

            if (response?.NextPage is null)
            {
                break;
            }

            page = response.NextPage.Value;
        }

        return new ArtistAlbums(
            ArtistId: artist.Id,
            ArtistName: artist.Name,
            Albums: albums.DistinctBy(album => album.Id).ToArray());
    }

    private async Task<ResolvedArtist?> ResolveArtistAsync(string artistName, CancellationToken cancellationToken)
    {
        var endpoint = $"/search?q={Uri.EscapeDataString(artistName)}";
        var payload = await GetAndDeserializeAsync<GeniusSearchEnvelope>(endpoint, cancellationToken);

        var candidates = payload.Response?.Hits?
            .Select(hit => hit.Result?.PrimaryArtist)
            .Where(artist => artist is not null)
            .Select(artist => new ResolvedArtist(artist!.Id, artist.Name))
            .DistinctBy(artist => artist.Id)
            .ToList() ?? [];

        if (candidates.Count == 0)
        {
            return null;
        }

        var exactMatch = candidates.FirstOrDefault(candidate =>
            string.Equals(candidate.Name, artistName, StringComparison.OrdinalIgnoreCase));

        return exactMatch ?? candidates[0];
    }

    private async Task<TPayload> GetAndDeserializeAsync<TPayload>(string endpoint, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException(
                "Genius API rejected the configured access token. Check GeniusAPI:clientAccessToken.");
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<TPayload>(stream, JsonOptions, cancellationToken);

        if (payload is null)
        {
            throw new InvalidOperationException($"Unable to parse Genius API response from endpoint '{endpoint}'.");
        }

        return payload;
    }

    private static ArtistAlbum MapAlbum(GeniusAlbumDto album)
    {
        return new ArtistAlbum(
            Id: album.Id,
            Name: album.Name,
            Url: album.Url,
            CoverArtUrl: album.CoverArtUrl,
            ReleaseDate: ParseReleaseDate(album.ReleaseDate));
    }

    private static DateOnly? ParseReleaseDate(string? releaseDate)
    {
        if (string.IsNullOrWhiteSpace(releaseDate))
        {
            return null;
        }

        return DateOnly.TryParse(releaseDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : null;
    }

    private sealed record ResolvedArtist(long Id, string Name);

    private sealed class GeniusSearchEnvelope
    {
        [JsonPropertyName("response")]
        public GeniusSearchResponse? Response { get; init; }
    }

    private sealed class GeniusSearchResponse
    {
        [JsonPropertyName("hits")]
        public List<GeniusSearchHit>? Hits { get; init; }
    }

    private sealed class GeniusSearchHit
    {
        [JsonPropertyName("result")]
        public GeniusSearchResult? Result { get; init; }
    }

    private sealed class GeniusSearchResult
    {
        [JsonPropertyName("primary_artist")]
        public GeniusPrimaryArtist? PrimaryArtist { get; init; }
    }

    private sealed class GeniusPrimaryArtist
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;
    }

    private sealed class GeniusArtistAlbumsEnvelope
    {
        [JsonPropertyName("response")]
        public GeniusArtistAlbumsResponse? Response { get; init; }
    }

    private sealed class GeniusArtistAlbumsResponse
    {
        [JsonPropertyName("albums")]
        public List<GeniusAlbumDto>? Albums { get; init; }

        [JsonPropertyName("next_page")]
        public int? NextPage { get; init; }
    }

    private sealed class GeniusAlbumDto
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;

        [JsonPropertyName("cover_art_url")]
        public string? CoverArtUrl { get; init; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; init; }
    }
}
