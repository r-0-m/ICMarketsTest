using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Persistence.Options;

namespace ICMarketsTest.Infrastructure.Persistence.Stores;

public sealed class InMemorySnapshotStore : ISnapshotStore
{
    private readonly List<BlockchainSnapshotDto> _snapshots = new();
    private readonly object _lock = new();
    private readonly TimeSpan _minInterval;

    public InMemorySnapshotStore(SnapshotDedupOptions dedupOptions)
    {
        _minInterval = TimeSpan.FromSeconds(Math.Max(0, dedupOptions.MinIntervalSeconds));
    }

    public Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (!IsWithinInterval(snapshot, GetLatest(snapshot.Network)))
            {
                _snapshots.Add(snapshot);
            }
        }

        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            foreach (var group in snapshots.GroupBy(snapshot => snapshot.Network))
            {
                var latest = GetLatest(group.Key);
                foreach (var snapshot in group.OrderBy(snapshot => snapshot.CreatedAt))
                {
                    if (IsWithinInterval(snapshot, latest))
                    {
                        continue;
                    }

                    _snapshots.Add(snapshot);
                    latest = snapshot;
                }
            }
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

    private BlockchainSnapshotDto? GetLatest(string network)
    {
        return _snapshots
            .Where(snapshot => snapshot.Network == network)
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .FirstOrDefault();
    }

    private bool IsWithinInterval(BlockchainSnapshotDto snapshot, BlockchainSnapshotDto? latest)
    {
        if (_minInterval <= TimeSpan.Zero || latest is null)
        {
            return false;
        }

        return snapshot.CreatedAt <= latest.CreatedAt.Add(_minInterval);
    }
}
