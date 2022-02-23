using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Queries;
using FoosballScore.Api.Mapping;
using FoosballScore.Api.Models;
using FoosballScore.Api.Queries;
using FoosballScore.Api.ReadModels;
using FoosballScore.Domain;
using FoosballScore.Domain.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoosballScore.Api.Controllers;

[Route("api/games")]
[ResponseCache(NoStore = true)]
public class GameController : ControllerBase
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ICommandBus _commandBus;

    public GameController(IQueryProcessor queryProcessor, ICommandBus commandBus)
    {
        _queryProcessor = queryProcessor;
        _commandBus = commandBus;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<GameViewModel>>> GetGames(
        [FromQuery(Name = QueryStringParameters.CreatedAfterEpoch)] int? createdAfterEpoch,
        [FromQuery(Name = QueryStringParameters.CreatedBeforeEpoch)] int? createdBeforeEpoch,
        CancellationToken cancellationToken)
    {
        if (createdAfterEpoch.HasValue && createdBeforeEpoch.HasValue && createdBeforeEpoch.Value < createdAfterEpoch.Value)
        {
            ModelState.AddModelError(QueryStringParameters.CreatedAfterEpoch, "Negative date time range");
            ModelState.AddModelError(QueryStringParameters.CreatedBeforeEpoch, "Negative date time range");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        DateTimeOffset? createdAfter = createdAfterEpoch.HasValue ? DateTimeOffset.FromUnixTimeSeconds(createdAfterEpoch.Value) : null;
        DateTimeOffset? createdBefore = createdBeforeEpoch.HasValue ? DateTimeOffset.FromUnixTimeSeconds(createdBeforeEpoch.Value) : null;

        IReadOnlyCollection<GameReadModel> gameReadModels = await _queryProcessor
            .ProcessAsync(new GetGamesQuery(createdAfter, createdBefore), cancellationToken)
            .ConfigureAwait(false);

        if (gameReadModels.Count == 0)
        {
            return NoContent();
        }

        return Ok(gameReadModels.ToViewModel());
    }

    [HttpGet("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameViewModel>> GetGame(Guid id, CancellationToken cancellationToken)
    {
        GameReadModel? gameReadModel = await _queryProcessor
            .ProcessAsync(new GetGameQuery(id), cancellationToken)
            .ConfigureAwait(false);

        if (gameReadModel == null)
        {
            return Problem($"Game {id} does not exist", Request.Path, StatusCodes.Status404NotFound, "Game not found");
        }

        return Ok(gameReadModel.ToViewModel());
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameViewModel>> CreateGame(
        [FromQuery, Required, MinLength(Constraints.NameMinLength), MaxLength(Constraints.NameMaxLength)] string name,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        GameId id = GameId.NewComb();

        IExecutionResult result = await _commandBus
            .PublishAsync(new CreateGameCommand(id, name), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsSuccess)
        {
            GameReadModel? gameReadModel = await _queryProcessor
                .ProcessAsync(new GetGameQuery(id.GetGuid()), cancellationToken)
                .ConfigureAwait(false);

            return CreatedAtAction(nameof(GetGame), new { id = id.GetGuid() }, gameReadModel?.ToViewModel());
        }

        return Problem(result.ToString(), Request.Path, StatusCodes.Status500InternalServerError, "Failed to create game");
    }

    [HttpPost("{id:guid}/register-goal")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameViewModel>> RegisterGoal(
        [FromRoute, Required] Guid id,
        [FromQuery, Required(ErrorMessage = $"Valid values are {nameof(Team.Blue)} and {nameof(Team.Red)}")] Team? team,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        IExecutionResult result = await _commandBus
            .PublishAsync(new ScoreGoalCommand(GameId.With(id), team!.Value), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return AcceptedAtAction(nameof(GetGame), new { id }, null);
        }

        if (result is ErrorCodeFailedExecutionResult errorCodeResult)
        {
            if (errorCodeResult.ErrorCode == DomainErrors.GameNotFound)
            {
                return Problem($"Game {id} does not exist", Request.Path, StatusCodes.Status404NotFound, "Game not found");
            }

            if (errorCodeResult.ErrorCode == DomainErrors.GameAlreadyFinished)
            {
                return Problem($"Game {id} has already finished", Request.Path, StatusCodes.Status400BadRequest, "Game ended");
            }
        }

        return Problem(result.ToString(), Request.Path, StatusCodes.Status500InternalServerError, "Failed to register goal");
    }
}