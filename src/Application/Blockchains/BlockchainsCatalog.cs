namespace ICMarketsTest.Application.Blockchains;

/// <summary>
/// Catalog of supported BlockCypher networks.
/// </summary>
public static class BlockchainsCatalog
{
    public static readonly IReadOnlyList<BlockchainDefinition> All = new[]
    {
        new BlockchainDefinition("eth-main", "Ethereum Mainnet", "https://api.blockcypher.com/v1/eth/main"),
        new BlockchainDefinition("dash-main", "Dash Mainnet", "https://api.blockcypher.com/v1/dash/main"),
        new BlockchainDefinition("btc-main", "Bitcoin Mainnet", "https://api.blockcypher.com/v1/btc/main"),
        new BlockchainDefinition("btc-test3", "Bitcoin Testnet 3", "https://api.blockcypher.com/v1/btc/test3"),
        new BlockchainDefinition("ltc-main", "Litecoin Mainnet", "https://api.blockcypher.com/v1/ltc/main")
    };

    public static bool TryGet(string key, out BlockchainDefinition definition)
    {
        definition = All.FirstOrDefault(item =>
            string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))!;
        return definition is not null;
    }
}
