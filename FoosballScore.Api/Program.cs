using System;
using System.Text.Json.Serialization;
using System.Threading;
using EventFlow.Extensions;
using EventFlow.MsSql;
using EventFlow.MsSql.Extensions;
using FluentMigrator.Runner;
using FoosballScore.Api;
using FoosballScore.Api.ReadModels;
using FoosballScore.Api.Storage;
using FoosballScore.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
string databaseConnection = configuration.GetValue<string>("database-connection") ??
    throw new ApplicationException("Use --database-connection command line parameter to provide a valid connection string");

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEventFlow(options => options
    .ConfigureMsSql(MsSqlConfiguration.New.SetConnectionString(databaseConnection))
    .UseMssqlEventStore()
    .UseMssqlReadModel<GameReadModel>()
    .AddQueryHandlers(typeof(ApiLocator).Assembly)
    .AddDefaults(typeof(DomainLocator).Assembly));

builder.Services.AddFluentMigratorCore().ConfigureRunner(options => options
    .AddSqlServer()
    .WithGlobalConnectionString(databaseConnection)
    .ScanIn(typeof(ApiLocator).Assembly).For.Migrations());

builder.Services.AddDbContext<GameDbContext>(options => options
    .UseSqlServer(databaseConnection)
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddSingleton<StorageInitializer>();

await using WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

StorageInitializer storageInitializer = app.Services.GetRequiredService<StorageInitializer>();
await storageInitializer.Init(CancellationToken.None);

await app.RunAsync();