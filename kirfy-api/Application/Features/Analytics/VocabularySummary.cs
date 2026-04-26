namespace kirfy_api.Application.Features.Analytics;

public sealed record VocabularySummary(
    int TotalWords,
    int UniqueWords,
    double RepetitionRatio,
    double TypeTokenRatio);
