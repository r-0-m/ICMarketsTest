using ICMarketsTest.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ICMarketsTest.Infrastructure.Clients;

/// <summary>
/// HttpClient-based BlockCypher API client.
/// </summary>
public sealed class BlockCypherClient : IBlockCypherClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BlockCypherClient> _logger;
    private readonly TimeSpan _minDelay;
    private readonly SemaphoreSlim _throttle = new(1, 1);
    private DateTimeOffset _lastRequest = DateTimeOffset.MinValue;

    public BlockCypherClient(
        HttpClient httpClient,
        ILogger<BlockCypherClient> logger,
        BlockCypherClientOptions options)
    {
        _httpClient = httpClient;
        _logger = logger;
        var delayMs = Math.Max(0, options.MinDelayMilliseconds);
        _minDelay = TimeSpan.FromMilliseconds(delayMs);
    }

    public async Task<string> GetBlockchainAsync(string url, CancellationToken cancellationToken)
    {
        await ThrottleAsync(cancellationToken);
        _logger.LogInformation("Requesting BlockCypher data from {Url}", url);
        return await _httpClient.GetStringAsync(url, cancellationToken);
    }

    private async Task ThrottleAsync(CancellationToken cancellationToken)
    {
        await _throttle.WaitAsync(cancellationToken);
        try
        {
            var now = DateTimeOffset.UtcNow;
            var nextAllowed = _lastRequest + _minDelay;
            if (nextAllowed > now)
            {
                await Task.Delay(nextAllowed - now, cancellationToken);
            }

            _lastRequest = DateTimeOffset.UtcNow;
        }
        finally
        {
            _throttle.Release();
        }
    }
}
