using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using FoosballScore.Api.Queries;
using FoosballScore.Api.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FoosballScore.Api.QueryHandlers;

public class GetGameQueryHandler : IQueryHandler<GetGameQuery, GameReadModel?>
{
    private readonly GameDbContext _gameDbContext;

    public GetGameQueryHandler(GameDbContext gameDbContext)
    {
        _gameDbContext = gameDbContext;
    }

    public Task<GameReadModel?> ExecuteQueryAsync(GetGameQuery query, CancellationToken cancellationToken)
    {
        return _gameDbContext.Games.SingleOrDefaultAsync(game => game.Id == query.Id, cancellationToken);
    }
}