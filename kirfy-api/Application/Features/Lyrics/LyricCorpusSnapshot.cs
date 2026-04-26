namespace kirfy_api.Application.Features.Lyrics;

public sealed record LyricCorpusSnapshot(
    long ArtistId,
    string ArtistName,
    int SongsProcessed,
    int TotalWordCount);
