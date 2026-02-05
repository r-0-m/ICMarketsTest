using ICMarketsTest.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

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
    private DateTime _lastRequest = DateTime.MinValue;

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

        var delays = new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(750) };
        Exception? lastException = null;

        for (var attempt = 0; attempt <= delays.Length; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }

                if (response.StatusCode == (HttpStatusCode)429 ||
                    (int)response.StatusCode >= 500)
                {
                    if (attempt < delays.Length)
                    {
                        await Task.Delay(delays[attempt], cancellationToken);
                        continue;
                    }
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) when (attempt < delays.Length)
            {
                lastException = ex;
                await Task.Delay(delays[attempt], cancellationToken);
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }
        }

        throw lastException ?? new HttpRequestException("Failed to fetch BlockCypher data.");
    }

    private async Task ThrottleAsync(CancellationToken cancellationToken)
    {
        await _throttle.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            var nextAllowed = _lastRequest + _minDelay;
            if (nextAllowed > now)
            {
                await Task.Delay(nextAllowed - now, cancellationToken);
            }

            _lastRequest = DateTime.UtcNow;
        }
        finally
        {
            _throttle.Release();
        }
    }
}
