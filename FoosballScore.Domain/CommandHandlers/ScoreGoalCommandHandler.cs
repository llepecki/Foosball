using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using FoosballScore.Domain.Commands;

namespace FoosballScore.Domain.CommandHandlers;

public class ScoreGoalCommandHandler : ICommandHandler<Game, GameId, IExecutionResult, ScoreGoalCommand>
{
    public Task<IExecutionResult> ExecuteCommandAsync(Game game, ScoreGoalCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(game.ScoreGoal(command.Team));
}