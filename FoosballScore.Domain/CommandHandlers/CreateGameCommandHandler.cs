using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using FoosballScore.Domain.Commands;

namespace FoosballScore.Domain.CommandHandlers;

public class CreateGameCommandHandler : ICommandHandler<Game, GameId, IExecutionResult, CreateGameCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(Game game, CreateGameCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            Task.FromResult(ExecutionResult.Failed($"{nameof(Game.Name)} is required"));
        }

        if (command.Name.Length < Constraints.NameMinLength)
        {
            Task.FromResult(ExecutionResult.Failed($"{nameof(Game.Name)} has to be at least {Constraints.NameMinLength} characters long"));
        }

        if (command.Name.Length > Constraints.NameMaxLength)
        {
            Task.FromResult(ExecutionResult.Failed($"{nameof(Game.Name)} can be up to {Constraints.NameMaxLength} characters long"));
        }

        return Task.FromResult(game.CreateGame(command.Name));
    }
}