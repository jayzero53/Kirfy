namespace kirfy_api.Infrastructure.Persistence;

public sealed class KirfyPersistenceOptions
{
    public const string SectionName = "Persistence";
    public string ConnectionString { get; init; } = string.Empty;
}
