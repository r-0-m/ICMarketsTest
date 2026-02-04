namespace ICMarketsTest.Infrastructure.Entities;

/// <summary>
/// Persisted snapshot from a BlockCypher network call.
/// </summary>
public sealed class BlockchainSnapshot
{
    public Guid Id { get; set; }
    public string Network { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
