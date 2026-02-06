namespace ICMarketsTest.Infrastructure.Persistence.Options;

/// <summary>
/// Deduplication settings for snapshot ingestion.
/// </summary>
public sealed class SnapshotDedupOptions
{
    /// <summary>
    /// Minimum interval in seconds between stored snapshots per network.
    /// Use 0 to disable deduplication.
    /// </summary>
    public int MinIntervalSeconds { get; init; }
}
