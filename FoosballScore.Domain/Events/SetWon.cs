using EventFlow.Aggregates;

namespace FoosballScore.Domain.Events;

public class SetWon : AggregateEvent<Game, GameId>
{
    public SetWon(Team winner, int set)
    {
        Winner = winner;
        Set = set;
    }

    public Team Winner { get; }

    public int Set { get; }
}