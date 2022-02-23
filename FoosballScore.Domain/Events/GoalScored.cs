using EventFlow.Aggregates;

namespace FoosballScore.Domain.Events;

public class GoalScored : AggregateEvent<Game, GameId>
{
    public GoalScored(Team team, int set, int goals)
    {
        Team = team;
        Set = set;
        Goals = goals;
    }

    public Team Team { get; }

    public int Set { get; }

    public int Goals { get; }
}