using ICMarketsTest.Infrastructure.Data;
using ICMarketsTest.Infrastructure.Interfaces;

namespace ICMarketsTest.Infrastructure.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IBlockchainSnapshotRepository _snapshots;

    public UnitOfWork(AppDbContext dbContext, IBlockchainSnapshotRepository snapshots)
    {
        _dbContext = dbContext;
        _snapshots = snapshots;
    }

    public IBlockchainSnapshotRepository Snapshots => _snapshots;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
