namespace ICMarketsTest.Application.Blockchains;

/// <summary>
/// Defines a supported blockchain network and its BlockCypher endpoint.
/// </summary>
/// <param name="Key">Network key, e.g. btc-main.</param>
/// <param name="DisplayName">Human readable display name.</param>
/// <param name="Url">BlockCypher base URL.</param>
public sealed record BlockchainDefinition(string Key, string DisplayName, string Url);
