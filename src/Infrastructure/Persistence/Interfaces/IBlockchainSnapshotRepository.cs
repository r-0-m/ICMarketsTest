using ICMarketsTest.Infrastructure.Persistence.Entities;

namespace ICMarketsTest.Infrastructure.Persistence.Interfaces;

/// <summary>
/// Repository for blockchain snapshots.
/// </summary>
public interface IBlockchainSnapshotRepository
{
    Task AddAsync(BlockchainSnapshot snapshot, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<BlockchainSnapshot> snapshots, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlockchainSnapshot>> GetAsync(string? network, int? limit, CancellationToken cancellationToken);
    Task<BlockchainSnapshot?> GetLatestAsync(string network, CancellationToken cancellationToken);
}
