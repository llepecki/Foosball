using EventFlow.Aggregates;

namespace FoosballScore.Domain.Events;

public class GameCreated : AggregateEvent<Game, GameId>
{
    public GameCreated(string name, int currentSet)
    {
        Name = name;
        CurrentSet = currentSet;
    }

    public string Name { get; }

    public int CurrentSet { get; }
}