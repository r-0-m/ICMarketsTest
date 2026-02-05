using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Blockchains;
using ICMarketsTest.Core.Commands;
using ICMarketsTest.Core.Events;
using ICMarketsTest.Core.Interfaces;

namespace ICMarketsTest.Core.Handlers;

public sealed class SyncAllBlockchainsHandler
{
    private readonly IBlockCypherClient _blockCypherClient;
    private readonly ISnapshotStore _snapshotStore;
    private readonly IEventPublisher _eventPublisher;

    public SyncAllBlockchainsHandler(
        IBlockCypherClient blockCypherClient,
        ISnapshotStore snapshotStore,
        IEventPublisher eventPublisher)
    {
        _blockCypherClient = blockCypherClient;
        _snapshotStore = snapshotStore;
        _eventPublisher = eventPublisher;
    }

    public async Task<IReadOnlyList<BlockchainSnapshotDto>> HandleAsync(
        SyncAllBlockchainsCommand command,
        CancellationToken cancellationToken)
    {
        var fetchTasks = BlockchainsCatalog.All.Select(async definition =>
        {
            var payload = await TryFetchPayloadAsync(definition.Url, cancellationToken);
            return new BlockchainSnapshotDto
            {
                Id = Guid.NewGuid(),
                Network = definition.Key,
                SourceUrl = definition.Url,
                Payload = payload,
                CreatedAt = DateTime.UtcNow
            };
        });

        var snapshots = await Task.WhenAll(fetchTasks);
        await _snapshotStore.AddRangeAsync(snapshots, cancellationToken);

        foreach (var snapshot in snapshots)
        {
            await _eventPublisher.PublishAsync(new BlockchainSnapshotStored(snapshot), cancellationToken);
        }

        return snapshots;
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
