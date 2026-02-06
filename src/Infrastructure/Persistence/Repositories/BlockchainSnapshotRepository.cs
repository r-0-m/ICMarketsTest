using ICMarketsTest.Infrastructure.Persistence.Data;
using ICMarketsTest.Infrastructure.Persistence.Entities;
using ICMarketsTest.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ICMarketsTest.Infrastructure.Persistence.Repositories;

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

    public Task<BlockchainSnapshot?> GetLatestAsync(string network, CancellationToken cancellationToken)
    {
        return _dbContext.BlockchainSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.Network == network)
            .OrderByDescending(snapshot => snapshot.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
