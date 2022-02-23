using EventFlow.Core;

namespace FoosballScore.Domain;

public class GameId : Identity<GameId>
{
    public GameId(string value) : base(value)
    {
    }
}