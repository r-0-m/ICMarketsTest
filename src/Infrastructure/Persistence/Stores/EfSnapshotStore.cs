using AutoMapper;
using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Persistence.Entities;
using ICMarketsTest.Infrastructure.Persistence.Interfaces;

namespace ICMarketsTest.Infrastructure.Persistence.Stores;

/// <summary>
/// EF Core-backed snapshot store.
/// </summary>
public sealed class EfSnapshotStore : ISnapshotStore
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EfSnapshotStore(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<BlockchainSnapshot>(snapshot);
        await _unitOfWork.Snapshots.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        var entities = snapshots.Select(snapshot => _mapper.Map<BlockchainSnapshot>(snapshot)).ToList();
        await _unitOfWork.Snapshots.AddRangeAsync(entities, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockchainSnapshotDto>> GetAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.Snapshots.GetAsync(network, limit, cancellationToken);
        return entities.Select(entity => _mapper.Map<BlockchainSnapshotDto>(entity)).ToList();
    }
}
