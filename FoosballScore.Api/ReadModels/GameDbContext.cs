using Microsoft.EntityFrameworkCore;

namespace FoosballScore.Api.ReadModels;

public class GameDbContext : ReadOnlyDbContext
{
    public GameDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<GameReadModel> Games { get; protected init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameReadModel>().ToTable(nameof(GameReadModel));
    }
}