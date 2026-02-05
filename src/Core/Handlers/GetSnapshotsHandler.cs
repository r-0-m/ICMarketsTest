using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Core.Queries;

namespace ICMarketsTest.Core.Handlers;

public sealed class GetSnapshotsHandler
{
    private readonly ISnapshotStore _snapshotStore;

    public GetSnapshotsHandler(ISnapshotStore snapshotStore)
    {
        _snapshotStore = snapshotStore;
    }

    public Task<IReadOnlyList<BlockchainSnapshotDto>> HandleAsync(
        GetSnapshotsQuery query,
        CancellationToken cancellationToken)
    {
        return _snapshotStore.GetAsync(query.Network, query.Limit, cancellationToken);
    }
}
