using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Blockchains;
using ICMarketsTest.Core.Commands;
using ICMarketsTest.Core.Events;
using ICMarketsTest.Core.Interfaces;

namespace ICMarketsTest.Core.Handlers;

public sealed class SyncBlockchainHandler
{
    private readonly IBlockCypherClient _blockCypherClient;
    private readonly ISnapshotStore _snapshotStore;
    private readonly IEventPublisher _eventPublisher;

    public SyncBlockchainHandler(
        IBlockCypherClient blockCypherClient,
        ISnapshotStore snapshotStore,
        IEventPublisher eventPublisher)
    {
        _blockCypherClient = blockCypherClient;
        _snapshotStore = snapshotStore;
        _eventPublisher = eventPublisher;
    }

    public async Task<BlockchainSnapshotDto> HandleAsync(
        SyncBlockchainCommand command,
        CancellationToken cancellationToken)
    {
        if (!BlockchainsCatalog.TryGet(command.Network, out var definition))
        {
            throw new InvalidOperationException($"Unsupported blockchain network '{command.Network}'.");
        }

        var payload = await TryFetchPayloadAsync(definition.Url, cancellationToken);
        var snapshot = new BlockchainSnapshotDto
        {
            Id = Guid.NewGuid(),
            Network = definition.Key,
            SourceUrl = definition.Url,
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

        await _snapshotStore.AddAsync(snapshot, cancellationToken);
        await _eventPublisher.PublishAsync(new BlockchainSnapshotStored(snapshot), cancellationToken);
        return snapshot;
    }

    private async Task<string> TryFetchPayloadAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            return await _blockCypherClient.GetBlockchainAsync(url, cancellationToken);
        }
        catch
        {
            return "{\"status\":\"unavailable\"}";
        }
    }
}
