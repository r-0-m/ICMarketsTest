namespace ICMarketsTest.Api.Options;

public sealed class BlockchainsOptions
{
    public IReadOnlyList<BlockchainNetworkOptions> Networks { get; init; } = Array.Empty<BlockchainNetworkOptions>();
}

public sealed class BlockchainNetworkOptions
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
