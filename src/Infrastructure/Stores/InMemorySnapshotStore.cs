using ICMarketsTest.Application.Contracts;
using ICMarketsTest.Application.Interfaces;

namespace ICMarketsTest.Infrastructure.Stores;

public sealed class InMemorySnapshotStore : ISnapshotStore
{
    private readonly List<BlockchainSnapshotDto> _snapshots = new();
    private readonly object _lock = new();

    public Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _snapshots.Add(snapshot);
        }

        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _snapshots.AddRange(snapshots);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        IEnumerable<BlockchainSnapshotDto> query;
        lock (_lock)
        {
            query = _snapshots.ToList();
        }

        if (!string.IsNullOrWhiteSpace(network))
        {
            query = query.Where(snapshot => snapshot.Network == network);
        }

        query = query.OrderByDescending(snapshot => snapshot.CreatedAt);

        if (limit is > 0)
        {
            query = query.Take(limit.Value);
        }

        return Task.FromResult<IReadOnlyList<BlockchainSnapshotDto>>(query.ToList());
    }
}
