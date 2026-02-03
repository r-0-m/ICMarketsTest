using ICMarketsTest.Application.Contracts;

namespace ICMarketsTest.Application.Interfaces;

/// <summary>
/// Abstraction for storing and retrieving blockchain snapshots.
/// </summary>
public interface ISnapshotStore
{
    /// <summary>Adds a single snapshot.</summary>
    Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken);
    /// <summary>Adds multiple snapshots.</summary>
    Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken);
    /// <summary>Returns snapshots ordered by CreatedAt descending.</summary>
    Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(string? network, int? limit, CancellationToken cancellationToken);
}
