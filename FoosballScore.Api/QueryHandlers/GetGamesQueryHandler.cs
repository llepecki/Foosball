using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using FoosballScore.Api.Queries;
using FoosballScore.Api.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FoosballScore.Api.QueryHandlers;

public class GetGamesQueryHandler : IQueryHandler<GetGamesQuery, IReadOnlyCollection<GameReadModel>>
{
    private readonly GameDbContext _gameDbContext;

    public GetGamesQueryHandler(GameDbContext gameDbContext)
    {
        _gameDbContext = gameDbContext;
    }

    public async Task<IReadOnlyCollection<GameReadModel>> ExecuteQueryAsync(GetGamesQuery query, CancellationToken cancellationToken)
    {
        IQueryable<GameReadModel> queryable = _gameDbContext.Games;

        if (query.CreatedAfter.HasValue)
        {
            queryable = queryable.Where(game => game.Created > query.CreatedAfter.Value);
        }

        if (query.CreatedBefore.HasValue)
        {
            queryable = queryable.Where(game => game.Created < query.CreatedBefore.Value);
        }

        return await queryable.OrderByDescending(game => game.Created).ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }
}