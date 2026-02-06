using ICMarketsTest.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICMarketsTest.Infrastructure.Persistence.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BlockchainSnapshot> BlockchainSnapshots => Set<BlockchainSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockchainSnapshot>(entity =>
        {
            entity.HasKey(snapshot => snapshot.Id);
            entity.Property(snapshot => snapshot.Network).HasMaxLength(32).IsRequired();
            entity.Property(snapshot => snapshot.SourceUrl).HasMaxLength(256).IsRequired();
            entity.Property(snapshot => snapshot.Payload).IsRequired();
            entity.Property(snapshot => snapshot.CreatedAt).IsRequired();
            entity.HasIndex(snapshot => new { snapshot.Network, snapshot.CreatedAt });
        });
    }
}
