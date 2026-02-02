using ICMarketsTest.Application.Contracts;
using ICMarketsTest.Application.Interfaces;

namespace ICMarketsTest.Infrastructure.Stores;

public sealed class InMemorySnapshotStore : ISnapshotStore
{
    public Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
