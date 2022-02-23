using EventFlow.Aggregates;

namespace FoosballScore.Domain.Events;

public class GameWon : AggregateEvent<Game, GameId>
{
    public GameWon(Team winner, int set)
    {
        Winner = winner;
        Set = set;
    }

    public Team Winner { get; }

    public int Set { get; }
}