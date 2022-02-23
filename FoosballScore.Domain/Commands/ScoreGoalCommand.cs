using EventFlow.Commands;

namespace FoosballScore.Domain.Commands;

public class ScoreGoalCommand : Command<Game, GameId>
{
    public ScoreGoalCommand(GameId id, Team team) : base(id)
    {
        Team = team;
    }

    public Team Team { get; }
}