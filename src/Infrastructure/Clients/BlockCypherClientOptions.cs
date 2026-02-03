namespace ICMarketsTest.Infrastructure.Clients;

/// <summary>
/// Rate-limiting options for BlockCypher requests.
/// </summary>
public sealed class BlockCypherClientOptions
{
    /// <summary>
    /// Minimum delay between calls in milliseconds.
    /// </summary>
    public int MinDelayMilliseconds { get; init; } = 350;
}
