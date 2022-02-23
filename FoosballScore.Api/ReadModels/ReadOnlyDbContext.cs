namespace FoosballScore.Api.ReadModels;

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class ReadOnlyDbContext : DbContext
{
    protected ReadOnlyDbContext()
    {
    }

    protected ReadOnlyDbContext(DbContextOptions options) : base(options)
    {
    }

    public override int SaveChanges() =>
        throw new NotImplementedException($"{GetType().Name} is intended only for read operations");

    public override int SaveChanges(bool _1) =>
        throw new NotImplementedException($"{GetType().Name} is intended only for read operations");

    public override Task<int> SaveChangesAsync(CancellationToken _1 = new CancellationToken()) =>
        throw new NotImplementedException($"{GetType().Name} is intended only for read operations");

    public override Task<int> SaveChangesAsync(bool _1, CancellationToken _2 = new CancellationToken()) =>
        throw new NotImplementedException($"{GetType().Name} is intended only for read operations");
}