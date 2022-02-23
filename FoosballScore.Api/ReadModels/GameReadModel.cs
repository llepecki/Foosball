using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlow.Sql.ReadModels.Attributes;
using FoosballScore.Domain;
using FoosballScore.Domain.Events;

namespace FoosballScore.Api.ReadModels;

[Table(nameof(GameReadModel))]
public record GameReadModel :
    IReadModel,
    IAmReadModelFor<Game, GameId, GameCreated>,
    IAmReadModelFor<Game, GameId, GoalScored>,
    IAmReadModelFor<Game, GameId, SetWon>,
    IAmReadModelFor<Game, GameId, GameWon>
{
    [SqlReadModelIdentityColumn]
    public string AggregateId { get; protected set; } = null!;

    public Guid Id { get; protected set; }

    public string Name { get; protected set; } = null!;

    public int CurrentSet { get; protected set; }

    public string? FirstSetWinner { get; protected set; }

    public string? SecondSetWinner { get; protected set; }

    public string? ThirdSetWinner { get; protected set; }

    public int FirstSetBlueGoals { get; protected set; }

    public int FirstSetRedGoals { get; protected set; }

    public int SecondSetBlueGoals { get; protected set; }

    public int SecondSetRedGoals { get; protected set; }

    public int ThirdSetBlueGoals { get; protected set; }

    public int ThirdSetRedGoals { get; protected set; }

    public string? GameWinner { get; protected set; }

    public DateTimeOffset Created { get; protected set; }

    public DateTimeOffset Modified { get; protected set; }

    [SqlReadModelVersionColumn]
    public int Version { get; protected set; }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<Game, GameId, GameCreated> domainEvent, CancellationToken cancellationToken)
    {
        AggregateId = domainEvent.AggregateIdentity.Value;
        Id = domainEvent.AggregateIdentity.GetGuid();
        Name = domainEvent.AggregateEvent.Name;
        CurrentSet = domainEvent.AggregateEvent.CurrentSet;
        Created = Modified = domainEvent.Timestamp;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<Game, GameId, GoalScored> domainEvent, CancellationToken cancellationToken)
    {
        WriteGoals(domainEvent.AggregateEvent.Team, domainEvent.AggregateEvent.Set, domainEvent.AggregateEvent.Goals);
        CurrentSet = domainEvent.AggregateEvent.Set;
        Modified = domainEvent.Timestamp;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<Game, GameId, SetWon> domainEvent, CancellationToken cancellationToken)
    {
        WriteWinner(domainEvent.AggregateEvent.Winner, domainEvent.AggregateEvent.Set);
        CurrentSet = domainEvent.AggregateEvent.Set;
        Modified = domainEvent.Timestamp;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<Game, GameId, GameWon> domainEvent, CancellationToken cancellationToken)
    {
        WriteWinner(domainEvent.AggregateEvent.Winner);
        CurrentSet = domainEvent.AggregateEvent.Set;
        Modified = domainEvent.Timestamp;
        return Task.CompletedTask;
    }

    private void WriteGoals(Team team, int set, int goals)
    {
        ThrowIfSetOutOfRange(set);

        switch (set)
        {
            case 1 when team == Team.Blue:
                FirstSetBlueGoals = goals;
                break;

            case 1 when team == Team.Red:
                FirstSetRedGoals = goals;
                break;

            case 2 when team == Team.Blue:
                SecondSetBlueGoals = goals;
                break;

            case 2 when team == Team.Red:
                SecondSetRedGoals = goals;
                break;

            case 3 when team == Team.Blue:
                ThirdSetBlueGoals = goals;
                break;

            case 3 when team == Team.Red:
                ThirdSetRedGoals = goals;
                break;
        }
    }

    private void WriteWinner(Team team, int set)
    {
        ThrowIfSetOutOfRange(set);

        switch (set)
        {
            case 1:
                FirstSetWinner = team.ToString("G");
                break;

            case 2:
                SecondSetWinner = team.ToString("G");
                break;

            case 3:
                ThirdSetWinner = team.ToString("G");
                break;
        }
    }

    private void WriteWinner(Team team)
    {
        GameWinner = team.ToString("G");
    }

    private static void ThrowIfSetOutOfRange(int set)
    {
        if (set < 1 || set > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(set), set, null);
        }
    }
}