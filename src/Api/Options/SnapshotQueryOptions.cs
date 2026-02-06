namespace ICMarketsTest.Api.Options;

/// <summary>
/// Query limits for snapshot endpoints.
/// </summary>
public sealed class SnapshotQueryOptions
{
    /// <summary>Maximum allowed limit value.</summary>
    public int MaxLimit { get; init; }
}
