using ICMarketsTest.Infrastructure.Entities;
using ICMarketsTest.Infrastructure.Interfaces;
using ICMarketsTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ICMarketsTest.Infrastructure.Repositories;

public sealed class BlockchainSnapshotRepository : IBlockchainSnapshotRepository
{
    private readonly AppDbContext _dbContext;

    public BlockchainSnapshotRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(BlockchainSnapshot snapshot, CancellationToken cancellationToken)
    {
        return _dbContext.BlockchainSnapshots.AddAsync(snapshot, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<BlockchainSnapshot> snapshots, CancellationToken cancellationToken)
    {
        return _dbContext.BlockchainSnapshots.AddRangeAsync(snapshots, cancellationToken);
    }

    public async Task<IReadOnlyList<BlockchainSnapshot>> GetAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.BlockchainSnapshots.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(network))
        {
            query = query.Where(snapshot => snapshot.Network == network);
        }

        query = query.OrderByDescending(snapshot => snapshot.CreatedAt);

        if (limit is > 0)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
