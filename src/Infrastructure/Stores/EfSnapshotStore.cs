using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Entities;
using ICMarketsTest.Infrastructure.Interfaces;

namespace ICMarketsTest.Infrastructure.Stores;

/// <summary>
/// EF Core-backed snapshot store.
/// </summary>
public sealed class EfSnapshotStore : ISnapshotStore
{
    private readonly IUnitOfWork _unitOfWork;

    public EfSnapshotStore(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        var entity = MapToEntity(snapshot);
        await _unitOfWork.Snapshots.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        var entities = snapshots.Select(MapToEntity).ToList();
        await _unitOfWork.Snapshots.AddRangeAsync(entities, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.Snapshots.GetAsync(network, limit, cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    private static BlockchainSnapshot MapToEntity(BlockchainSnapshotDto snapshot)
    {
        return new BlockchainSnapshot
        {
            Id = snapshot.Id,
            Network = snapshot.Network,
            SourceUrl = snapshot.SourceUrl,
            Payload = snapshot.Payload,
            CreatedAt = snapshot.CreatedAt
        };
    }

    private static BlockchainSnapshotDto MapToDto(BlockchainSnapshot snapshot)
    {
        return new BlockchainSnapshotDto
        {
            Id = snapshot.Id,
            Network = snapshot.Network,
            SourceUrl = snapshot.SourceUrl,
            Payload = snapshot.Payload,
            CreatedAt = snapshot.CreatedAt
        };
    }
}
