namespace ICMarketsTest.Application.Contracts;

public sealed class BlockchainSnapshotDto
{
    public Guid Id { get; init; }
    public string Network { get; init; } = string.Empty;
    public string SourceUrl { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
