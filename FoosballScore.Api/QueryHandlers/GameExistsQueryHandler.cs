using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using FoosballScore.Api.Queries;
using FoosballScore.Api.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FoosballScore.Api.QueryHandlers;

public class GameExistsQueryHandler : IQueryHandler<GameExistsQuery, bool>
{
    private readonly GameDbContext _gameDbContext;

    public GameExistsQueryHandler(GameDbContext gameDbContext)
    {
        _gameDbContext = gameDbContext;
    }

    public async Task<bool> ExecuteQueryAsync(GameExistsQuery query, CancellationToken cancellationToken)
    {
        return await _gameDbContext.Games
            .Select(game => game.Id)
            .CountAsync(cancellationToken)
            .ConfigureAwait(false) == 1;
    }
}