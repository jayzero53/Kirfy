namespace kirfy_api.Domain.ValueObjects;

public sealed record WordFrequency(
    string Word,
    int Count,
    double NormalizedFrequency);
