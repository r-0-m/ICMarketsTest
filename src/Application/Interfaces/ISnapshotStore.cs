using ICMarketsTest.Application.Contracts;

namespace ICMarketsTest.Application.Interfaces;

public interface ISnapshotStore
{
    Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(string? network, int? limit, CancellationToken cancellationToken);
}
