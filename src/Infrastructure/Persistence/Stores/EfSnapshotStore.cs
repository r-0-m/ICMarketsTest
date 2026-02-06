using AutoMapper;
using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Persistence.Entities;
using ICMarketsTest.Infrastructure.Persistence.Interfaces;
using ICMarketsTest.Infrastructure.Persistence.Options;

namespace ICMarketsTest.Infrastructure.Persistence.Stores;

/// <summary>
/// EF Core-backed snapshot store.
/// </summary>
public sealed class EfSnapshotStore : ISnapshotStore
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeSpan _minInterval;

    public EfSnapshotStore(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        SnapshotDedupOptions dedupOptions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _minInterval = TimeSpan.FromSeconds(Math.Max(0, dedupOptions.MinIntervalSeconds));
    }

    public async Task AddAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        if (await ShouldSkipAsync(snapshot, cancellationToken))
        {
            return;
        }

        var entity = _mapper.Map<BlockchainSnapshot>(snapshot);
        await _unitOfWork.Snapshots.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<BlockchainSnapshotDto> snapshots, CancellationToken cancellationToken)
    {
        if (_minInterval <= TimeSpan.Zero)
        {
            var entities = snapshots.Select(snapshot => _mapper.Map<BlockchainSnapshot>(snapshot)).ToList();
            if (entities.Count == 0)
            {
                return;
            }

            await _unitOfWork.Snapshots.AddRangeAsync(entities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var entitiesToAdd = new List<BlockchainSnapshot>();
        foreach (var group in snapshots.GroupBy(snapshot => snapshot.Network))
        {
            var latest = await _unitOfWork.Snapshots.GetLatestAsync(group.Key, cancellationToken);
            foreach (var snapshot in group.OrderBy(snapshot => snapshot.CreatedAt))
            {
                if (IsWithinInterval(snapshot.CreatedAt, latest?.CreatedAt))
                {
                    continue;
                }

                var entity = _mapper.Map<BlockchainSnapshot>(snapshot);
                entitiesToAdd.Add(entity);
                latest = entity;
            }
        }

        if (entitiesToAdd.Count == 0)
        {
            return;
        }

        await _unitOfWork.Snapshots.AddRangeAsync(entitiesToAdd, cancellationToken);
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

    private async Task<bool> ShouldSkipAsync(BlockchainSnapshotDto snapshot, CancellationToken cancellationToken)
    {
        if (_minInterval <= TimeSpan.Zero)
        {
            return false;
        }

        var latest = await _unitOfWork.Snapshots.GetLatestAsync(snapshot.Network, cancellationToken);
        return IsWithinInterval(snapshot.CreatedAt, latest?.CreatedAt);
    }

    private bool IsWithinInterval(DateTime createdAt, DateTime? latestCreatedAt)
    {
        if (!latestCreatedAt.HasValue)
        {
            return false;
        }

        return createdAt <= latestCreatedAt.Value.Add(_minInterval);
    }
}
