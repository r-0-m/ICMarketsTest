namespace ICMarketsTest.Application.Interfaces;

/// <summary>
/// Abstraction for BlockCypher API calls.
/// </summary>
public interface IBlockCypherClient
{
    /// <summary>Fetches raw JSON for the given BlockCypher URL.</summary>
    Task<string> GetBlockchainAsync(string url, CancellationToken cancellationToken);
}
