using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.MsSql;
using EventFlow.MsSql.EventStores;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace FoosballScore.Api.Storage;

internal class StorageInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public StorageInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Init(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();

        scope.ServiceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();

        IMsSqlDatabaseMigrator eventFlowMigrator = scope.ServiceProvider
            .GetRequiredService<IMsSqlDatabaseMigrator>();

        await EventFlowEventStoresMsSql.MigrateDatabaseAsync(eventFlowMigrator, cancellationToken);
    }
}