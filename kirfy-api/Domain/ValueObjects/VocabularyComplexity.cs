namespace kirfy_api.Domain.ValueObjects;

public sealed record VocabularyComplexity(int TotalWords, int UniqueWords)
{
    public double TypeTokenRatio => TotalWords == 0 ? 0 : (double)UniqueWords / TotalWords;
    public double RepetitionRatio => 1 - TypeTokenRatio;
}
