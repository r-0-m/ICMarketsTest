using ICMarketsTest.Infrastructure.Entities;

namespace ICMarketsTest.Infrastructure.Interfaces;

/// <summary>
/// Repository for blockchain snapshots.
/// </summary>
public interface IBlockchainSnapshotRepository
{
    Task AddAsync(BlockchainSnapshot snapshot, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<BlockchainSnapshot> snapshots, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlockchainSnapshot>> GetAsync(string? network, int? limit, CancellationToken cancellationToken);
}
