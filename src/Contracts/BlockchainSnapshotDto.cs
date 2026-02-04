namespace ICMarketsTest.Contracts;

/// <summary>
/// Snapshot payload from a BlockCypher network call.
/// </summary>
public sealed class BlockchainSnapshotDto
{
    /// <summary>Snapshot identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Network key, e.g. btc-main.</summary>
    /// <example>btc-main</example>
    public string Network { get; init; } = string.Empty;

    /// <summary>Source BlockCypher URL.</summary>
    /// <example>https://api.blockcypher.com/v1/btc/main</example>
    public string SourceUrl { get; init; } = string.Empty;

    /// <summary>Raw JSON payload from BlockCypher.</summary>
    public string Payload { get; init; } = string.Empty;

    /// <summary>UTC timestamp when the snapshot was stored.</summary>
    public DateTime CreatedAt { get; init; }
}
