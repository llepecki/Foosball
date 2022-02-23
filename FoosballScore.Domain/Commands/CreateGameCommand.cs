using EventFlow.Commands;

namespace FoosballScore.Domain.Commands;

public class CreateGameCommand : Command<Game, GameId>
{
    public CreateGameCommand(GameId id, string name) : base(id) => Name = name;

    public string Name { get; }
}